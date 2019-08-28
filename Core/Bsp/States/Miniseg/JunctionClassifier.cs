using System.Collections.Generic;
using Helion.Bsp.Geometry;
using Helion.Util.Geometry;
using NLog;
using static Helion.Util.Assertion.Assert;

namespace Helion.Bsp.States.Miniseg
{
    /// <summary>
    /// The main helper class which tracks all the junctions in the map and is
    /// used to check if a point is inside the map or not. This is required for
    /// miniseg generation.
    /// </summary>
    public class JunctionClassifier
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<int, Junction> m_vertexToJunction = new Dictionary<int, Junction>();

        /// <summary>
        /// To be called every time a one sided segment is discovered when 
        /// reading the valid map entry collection. After that, then the
        /// <see cref="NotifyDoneAdding"/> should be invoked.
        /// </summary>
        /// <remarks>
        /// This does not make any junctions, that has to be held off until
        /// later for both code cleanliness and performance reasons.
        /// </remarks>
        /// <param name="segment">The segment to track.</param>
        public void Add(BspSegment segment)
        {
            if (!segment.OneSided)
                return;

            if (!m_vertexToJunction.TryGetValue(segment.EndIndex, out Junction? endJunction))
            {
                endJunction = new Junction();
                m_vertexToJunction.Add(segment.EndIndex, endJunction);
            }

            Precondition(!endJunction.InboundSegments.Contains(segment), "Adding same segment to inbound junction twice");
            endJunction.InboundSegments.Add(segment);
            
            if (!m_vertexToJunction.TryGetValue(segment.StartIndex, out Junction? startJunction))
            {
                startJunction = new Junction();
                m_vertexToJunction.Add(segment.StartIndex, startJunction);
            }

            Precondition(!startJunction.OutboundSegments.Contains(segment), "Adding same segment to outbound junction twice");
            startJunction.OutboundSegments.Add(segment);
        }

        /// <summary>
        /// Tells the junction classifier that we will not be adding anymore
        /// junctions from <see cref="Add"/> anymore. It will
        /// then compile all the junctions.
        /// </summary>
        /// <remarks>
        /// The only way to add new junctions after calling this should be
        /// through <see cref="AddSplitJunction"/>. This is unfortunately
        /// needed since creating the junctions on the fly while adding new
        /// segments would be extra work and extra code. It might be worth
        /// doing one day however since it is a code smell due to requiring
        /// users to know about this function.
        /// </remarks>
        public void NotifyDoneAdding()
        {
            // TODO: Would like to not have this, it requires the user to know it
            // which is bad design unless we have to for optimization reasons.
            foreach (var vertexJunctionPair in m_vertexToJunction)
            {
                int index = vertexJunctionPair.Key;
                Junction junction = vertexJunctionPair.Value;
                
                if (junction.HasUnexpectedSegCount())
                    Log.Warn("BSP junction at index {0} has wrong amount of one-sided lines, BSP tree likely to be malformed", index);

                junction.GenerateWedges();
            }
        }

        /// <summary>
        /// A function called during BSP partitioning where we create a new 
        /// junction when we split a one sided line in two.
        /// </summary>
        /// <param name="inboundSegment">The inbound segment.</param>
        /// <param name="outboundSegment">The outbound segment.</param>
        public void AddSplitJunction(BspSegment inboundSegment, BspSegment outboundSegment)
        {
            Precondition(!ReferenceEquals(inboundSegment, outboundSegment), "Trying to add the same segment as an inbound/outbound junction");
            int middleVertexIndex = inboundSegment.EndIndex;
            Precondition(outboundSegment.StartIndex == middleVertexIndex, "Adding split junction where inbound/outbound segs are not connected");
            Precondition(!m_vertexToJunction.ContainsKey(middleVertexIndex), "When creating a split, the middle vertex shouldn't already exist as a junction");

            // We create new junctions because this function is called from a
            // newly created split. This means the middle vertex is new and the
            // junction cannot exist by virtue of the pivot point never having
            // existed.
            Junction junction = new Junction();
            m_vertexToJunction[middleVertexIndex] = junction;

            junction.InboundSegments.Add(inboundSegment);
            junction.OutboundSegments.Add(outboundSegment);
            junction.AddWedge(inboundSegment, outboundSegment);
        }

        /// <summary>
        /// Checks if the indices from the values provided cross the void or 
        /// not, where the void is the space outside the map.
        /// </summary>
        /// <param name="firstIndex">The index of the first segment.</param>
        /// <param name="secondVertex">The actual vertex coordinate of the
        /// second segment.</param>
        /// <returns>True if the two vertices are crossing the void, false if
        /// it is inside the map.</returns>
        public bool CheckCrossingVoid(int firstIndex, Vec2D secondVertex)
        {
            if (m_vertexToJunction.TryGetValue(firstIndex, out Junction? junction))
                return !junction.BetweenWedge(secondVertex);
            return false;
        }
    }
}
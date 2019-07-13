﻿using Helion.Bsp.Geometry;
using Helion.Bsp.States.Convex;
using Helion.Util.Geometry;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static Helion.Util.Assertion.Assert;

namespace Helion.Bsp.Node
{
    /// <summary>
    /// An edge of a subsector, or better put: a segment on the edge of a 
    /// convex polygon that is the leaf of a BSP tree.
    /// </summary>
    public class SubsectorEdge : Seg2DBase
    {
        /// <summary>
        /// The unique identifier for no subsector.
        /// </summary>
        /// <remarks>
        /// By definition of a subsector, it must have at least one edge that
        /// is not a miniseg. A mathematical proof demonstrates that it's not
        /// possible to have a sector composed entirely of minisegs. A 
        /// corollary of this is that SectorId should never be equal to the
        /// value of this in the final BSP tree.
        /// </remarks>
        public const int NoSectorId = -1;

        /// <summary>
        /// The line that this edge is part of. This is a miniseg if it equals
        /// <see cref="BspSegment.MinisegLineId"/>.
        /// </summary>
        public readonly int LineId;

        /// <summary>
        /// The sector ID for this subsector. It can be missing and be equal to
        /// <see cref="NoSectorId"/> if it is a miniseg.
        /// </summary>
        public readonly int? SectorId;

        /// <summary>
        /// If this segment is on the front of the line or not. This is not
        /// meaningful if it is a miniseg, and can be either true or false.
        /// </summary>
        public readonly bool IsFront;

        /// <summary>
        /// True if it's a miniseg, false if not.
        /// </summary>
        public bool IsMiniseg => LineId == BspSegment.MinisegLineId;

        /// <summary>
        /// Creates a subsector edge from a miniseg start/end point.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        public SubsectorEdge(Vec2D start, Vec2D end) : 
            this(start, end, true, BspSegment.MinisegLineId, NoSectorId)
        {
        }

        public SubsectorEdge(Vec2D start, Vec2D end, bool front, int lineId, int sectorId) : 
            base(start, end)
        {
            IsFront = front;
            LineId = lineId;
            if (sectorId != NoSectorId)
                SectorId = sectorId;
        }

        /// <summary>
        /// Gets the sector ID, and whether this is running along the front
        /// side of the line (true) or the back side (false).
        /// </summary>
        /// <param name="segment">The segment that was formed.</param>
        /// <param name="originatingEndpoint">The endpoint we are starting to
        /// traverse along.</param>
        /// <param name="sectorLine">The line with sector info,</param>
        /// <param name="rotation">The rotation of the segment traversal.
        /// </param>
        /// <returns>A pair of a sector ID and a boolean status for whether we
        /// are on the front (true) or back side (false) of the line.</returns>
        private static (int, bool) GetSectorAndSideFrom(BspSegment segment, Endpoint originatingEndpoint,
            SectorLine sectorLine, Rotation rotation)
        {
            // Note that for this method, it is possible for a traversal to go
            // in a direction that would traverse the back side of a one-sided
            // line segment and still be a proper traversal.
            //
            // This occurs because the random segment chooser may pick a two
            // sided line segment at the start, and if so then it has to 
            // arbitrarily pick a direction to go.
            //
            // This direction cannot be known whether it is the correct 
            // direction or not (ex: if it picks a vertical line, do we know
            // if we're on the left side of a convex polygon or the right side?
            // We don't without analyzing lines around it).
            //
            // Because of this (and to keep the code simple), it will go in an
            // arbitrary direction and reverse the segments later on if needed.
            // Therefore we can assume that if we hit a one sided segment then
            // it's okay to assume we're grabbing the correct side since the
            // user should not be providing a malformed map.
            if (segment.OneSided)
                return (sectorLine.FrontSectorId, true);

            // If we're moving along with the line...
            if (originatingEndpoint == Endpoint.Start)
            {
                if (rotation == Rotation.Right)
                    return (sectorLine.FrontSectorId, true);
                return (sectorLine.BackSectorId, false);
            }
            
            if (rotation == Rotation.Right)
                return (sectorLine.BackSectorId, false);
            return (sectorLine.FrontSectorId, true);
        }

        private static List<SubsectorEdge> CreateSubsectorEdges(ConvexTraversal convexTraversal, 
            IList<SectorLine> lineToSectors, Rotation rotation)
        {
            List<ConvexTraversalPoint> traversal = convexTraversal.Traversal;
            Precondition(traversal.Count >= 3, "Traversal must yield at least a triangle in size");

            List<SubsectorEdge> subsectorEdges = new List<SubsectorEdge>();

            ConvexTraversalPoint firstTraversal = traversal.First();
            Vec2D startPoint = firstTraversal.ToPoint();
            foreach (ConvexTraversalPoint traversalPoint in traversal)
            {
                BspSegment segment = traversalPoint.Segment;
                Endpoint originatingEndpoint = traversalPoint.Endpoint;
                Vec2D endingPoint = segment.Opposite(traversalPoint.Endpoint);

                if (segment.IsMiniseg)
                    subsectorEdges.Add(new SubsectorEdge(startPoint, endingPoint));
                else
                {
                    Precondition(segment.LineId < lineToSectors.Count, "Segment has bad line ID or line to sectors list is invalid");

                    SectorLine sectorLine = lineToSectors[segment.LineId];
                    (int sectorId, bool isFront) = GetSectorAndSideFrom(segment, originatingEndpoint, sectorLine, rotation);
                    subsectorEdges.Add(new SubsectorEdge(startPoint, endingPoint, isFront, segment.LineId, sectorId));
                }

                startPoint = endingPoint;
            }

            Postcondition(subsectorEdges.Count == traversal.Count, "Added too many subsector edges in traversal");
            return subsectorEdges;
        }

        /// <summary>
        /// Reverses the edges of the list provided. This will also mutate the
        /// list so it will have all new reversed edges.
        /// </summary>
        /// <param name="edges">The edges to reverse.</param>
        private static void ReverseEdgesMutate(List<SubsectorEdge> edges)
        {
            List<SubsectorEdge> reversedEdges = new List<SubsectorEdge>();

            edges.Reverse();
            edges.ForEach(edge =>
            {
                int sectorId = edge.SectorId ?? NoSectorId;
                reversedEdges.Add(new SubsectorEdge(edge.End, edge.Start, edge.IsFront, edge.LineId, sectorId));
            });

            edges.Clear();
            edges.AddRange(reversedEdges);
        }

        [Conditional("DEBUG")]
        private static void AssertValidSubsectorEdges(List<SubsectorEdge> edges)
        {
            Precondition(edges.Count >= 3, "Not enough edges");

            int lastCorrectSector = NoSectorId;

            // According to https://stackoverflow.com/questions/204505/preserving-order-with-linq
            // the order is preserved for a Where() invocation, so we do not 
            // need to worry about order being scrambled.
            foreach (SubsectorEdge edge in edges.Where(e => e.SectorId.HasValue))
            {
                if (lastCorrectSector == NoSectorId)
                    lastCorrectSector = edge.SectorId ?? NoSectorId;
                else
                    Precondition(edge.SectorId == lastCorrectSector, "Subsector references multiple sectors");
            }

            Precondition(lastCorrectSector != NoSectorId, "Unable to find a sector for the subsector (entire sector is minisegs?)");
        }

        /// <summary>
        /// Creates a list of subsector edges from a convex traversal. This
        /// must only be called on what would be a valid convex subsector.
        /// </summary>
        /// <param name="convexTraversal">The traversal we did for a convex
        /// subsector.</param>
        /// <param name="lineToSectors">The mapping of line IDs as an index to
        /// the sector information.</param>
        /// <param name="rotation">The convex traversal rotation.</param>
        /// <returns>A list of subsector edges.</returns>
        public static IList<SubsectorEdge> FromClockwiseTraversal(ConvexTraversal convexTraversal, 
            IList<SectorLine> lineToSectors, Rotation rotation)
        {
            List<SubsectorEdge> edges = CreateSubsectorEdges(convexTraversal, lineToSectors, rotation);
            if (rotation != Rotation.Left)
                ReverseEdgesMutate(edges);

            AssertValidSubsectorEdges(edges);
            return edges;
        }
    }
}

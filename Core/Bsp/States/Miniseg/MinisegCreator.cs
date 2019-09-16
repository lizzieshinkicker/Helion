using System.Collections.Generic;
using Helion.Bsp.Geometry;
using Helion.Util.Geometry.Vectors;
using static Helion.Util.Assertion.Assert;

namespace Helion.Bsp.States.Miniseg
{
    public class MinisegCreator
    {
        public MinisegStates States { get; private set; } = new MinisegStates();
        protected readonly VertexAllocator VertexAllocator;
        protected readonly SegmentAllocator SegmentAllocator;
        protected readonly JunctionClassifier JunctionClassifier;

        public MinisegCreator(VertexAllocator vertexAllocator, SegmentAllocator segmentAllocator, 
            JunctionClassifier junctionClassifier)
        {
            VertexAllocator = vertexAllocator;
            SegmentAllocator = segmentAllocator;
            JunctionClassifier = junctionClassifier;
        }
        
        public void Load(BspSegment splitter, HashSet<int> collinearVertices)
        {
            Precondition(collinearVertices.Count >= 2, "Requires two or more vertices for miniseg generation (the splitter should have contributed two)");

            States = new MinisegStates();

            foreach (int vertexIndex in collinearVertices)
            {
                double splitterTime = splitter.ToTime(VertexAllocator[vertexIndex]);
                States.Vertices.Add(new VertexSplitterTime(vertexIndex, splitterTime));
            }

            States.Vertices.Sort();
        }

        public void Execute()
        {
            Precondition(States.State != MinisegState.Finished, "Trying to do miniseg generation when already finished");
            Precondition(States.CurrentVertexListIndex + 1 < States.Vertices.Count, "Overflow of vertex sliding window");

            VertexSplitterTime first = States.Vertices[States.CurrentVertexListIndex];
            VertexSplitterTime second = States.Vertices[States.CurrentVertexListIndex + 1];
            States.CurrentVertexListIndex++;

            HandleMinisegGeneration(first, second);

            bool isDone = (States.CurrentVertexListIndex + 1 >= States.Vertices.Count);
            States.State = (isDone ? MinisegState.Finished : MinisegState.Working);
        }

        protected void HandleMinisegGeneration(VertexSplitterTime first, VertexSplitterTime second)
        {
            States.VoidStatus = VoidStatus.NotInVoid;

            // If a segment exists for the vertices then we're walking along a
            // segment that was collinear with the splitter, so we don't need a
            // miniseg.
            if (SegmentAllocator.ContainsSegment(first.Index, second.Index))
                return;

            Vec2D secondVertex = VertexAllocator[second.Index];
            if (JunctionClassifier.CheckCrossingVoid(first.Index, secondVertex))
                States.VoidStatus = VoidStatus.InVoid;
            else
            {
                BspSegment miniseg = SegmentAllocator.GetOrCreate(first.Index, second.Index);
                States.Minisegs.Add(miniseg);
            }
        }
    }
}
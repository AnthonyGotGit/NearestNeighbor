
namespace NNC
{
    internal class KdTreeService
    {
        private VehicleKdTreeNode BuildKdTree(List<VehicleKdTreeNode> data, int depth)
        {
            if (data.Count == 0)
                return null;

            int axis = depth % 2; // Alternating between Latitude and Longitude

            // Find the median without sorting the entire list
            int medianIndex = data.Count / 2;
            SelectMedian(data, axis, 0, data.Count - 1, medianIndex);

            VehicleKdTreeNode node = data[medianIndex];

            node.Left = BuildKdTree(data.GetRange(0, medianIndex), depth + 1);
            node.Right = BuildKdTree(data.GetRange(medianIndex + 1, data.Count - medianIndex - 1), depth + 1);

            return node;
        }

        private void SelectMedian(List<VehicleKdTreeNode> data, int axis, int left, int right, int k)
        {
            if (left < right)
            {
                int pivotIndex = Partition(data, axis, left, right);
                if (k < pivotIndex)
                {
                    SelectMedian(data, axis, left, pivotIndex - 1, k);
                }
                else if (k > pivotIndex)
                {
                    SelectMedian(data, axis, pivotIndex + 1, right, k);
                }
            }
        }
        private int Partition(List<VehicleKdTreeNode> data, int axis, int left, int right)
        {
            int pivotIndex = left;
            VehicleKdTreeNode pivot = data[pivotIndex];

            while (left < right)
            {
                while (left < right && Compare(data[right], pivot, axis) >= 0)
                    right--;

                if (left < right)
                {
                    data[left] = data[right];
                    left++;
                }

                while (left < right && Compare(data[left], pivot, axis) <= 0)
                    left++;

                if (left < right)
                {
                    data[right] = data[left];
                    right--;
                }
            }

            data[left] = pivot;
            return left;
        }

        private int Compare(VehicleKdTreeNode a, VehicleKdTreeNode b, int axis)
        {
            if (axis == 0)
            {
                return a.Latitude.CompareTo(b.Latitude);
            }
            else
            {
                return a.Longitude.CompareTo(b.Longitude);
            }
        }

        public VehicleKdTreeNode BuildKdTree(List<VehicleKdTreeNode> data)
        {
            return BuildKdTree(data, 0);
        }


        public VehicleKdTreeNode SearchKdTree(VehicleKdTreeNode root, float targetLatitude, float targetLongitude)
        {
            return SearchKdTree(root, targetLatitude, targetLongitude, root, 0);
        }

        private VehicleKdTreeNode SearchKdTree(VehicleKdTreeNode node, float targetLatitude, float targetLongitude, VehicleKdTreeNode best, int depth)
        {
            if (node == null)
                return best;

            double nodeLatitude = node.Latitude;
            double nodeLongitude = node.Longitude;

            // Calculate the distance from the target point to the current node
            double distance = Math.Sqrt(Math.Pow(targetLatitude - nodeLatitude, 2) + Math.Pow(targetLongitude - nodeLongitude, 2));

            // If the current node is closer than the best node so far, update the best node.
            if (distance < Math.Sqrt(Math.Pow(targetLatitude - best.Latitude, 2) + Math.Pow(targetLongitude - best.Longitude, 2)))
            {
                best = node;
            }

            int axis = depth % 2; // Alternating between Latitude and Longitude

            // Recursively search left or right subtrees based on the splitting axis
            if (axis == 0)
            {
                // If the target is to the left, explore left subtree first.
                // If the target is to the right, explore right subtree first.
                if (targetLatitude < nodeLatitude)
                {
                    // Explore Left subtree
                    best = SearchKdTree(node.Left, targetLatitude, targetLongitude, best, depth + 1);

                    // Check if it's necessary to explore the right subtree.
                    if (targetLatitude + Math.Sqrt(Math.Pow(targetLatitude - best.Latitude, 2)) >= nodeLatitude)
                    {
                        best = SearchKdTree(node.Right, targetLatitude, targetLongitude, best, depth + 1);
                    }
                }
                else
                {
                    // Explore Right subtree
                    best = SearchKdTree(node.Right, targetLatitude, targetLongitude, best, depth + 1);

                    // Check if it's necessary to explore the left subtree.
                    if (targetLatitude - Math.Sqrt(Math.Pow(targetLatitude - best.Latitude, 2)) < nodeLatitude)
                    {
                        best = SearchKdTree(node.Left, targetLatitude, targetLongitude, best, depth + 1);
                    }
                }
            }
            else
            {
                // If the target is to the left, explore left subtree first.
                // If the target is to the right, explore right subtree first.
                if (targetLongitude < nodeLongitude)
                {
                    // Explore Left subtree
                    best = SearchKdTree(node.Left, targetLatitude, targetLongitude, best, depth + 1);

                    // Check if it's necessary to explore the right subtree.
                    if (targetLongitude + Math.Sqrt(Math.Pow(targetLongitude - best.Longitude, 2)) >= nodeLongitude)
                    {
                        best = SearchKdTree(node.Right, targetLatitude, targetLongitude, best, depth + 1);
                    }
                }
                else
                {
                    // Explore Right subtree
                    best = SearchKdTree(node.Right, targetLatitude, targetLongitude, best, depth + 1);

                    // Check if it's necessary to explore the left subtree.
                    if (targetLongitude - Math.Sqrt(Math.Pow(targetLongitude - best.Longitude, 2)) < nodeLongitude)
                    {
                        best = SearchKdTree(node.Left, targetLatitude, targetLongitude, best, depth + 1);
                    }
                }
            }
            return best;
        }
    }
}

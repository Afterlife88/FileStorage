//namespace FileStorage.Utils
//{
//    public static class ValidationHelpers
//    {
//        public static bool ValidateNode(this ModelState modelState, Node node, ICurrentUser currentUser)
//        {
//            if (node == null)
//            {
//                modelState.ErrorMessage = "The item is not found";

//                return modelState.IsValid;
//            }

//            if (IsUserHasAccess(node, currentUser) == false)
//            {
//                modelState.ErrorMessage = "Access denied";

//                return modelState.IsValid;
//            }

//            return modelState.IsValid;
//        }
//        private static bool IsUserHasAccess(Node node, ICurrentUser currentUser)
//        {
//            return node.OwnerId == currentUser.Id;
//        }
//    }
//}

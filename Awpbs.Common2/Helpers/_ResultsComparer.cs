//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Awpbs
//{
//    public class ResultsComparer
//    {
//        public int Compare(Result result1, Result result2)
//        {
//            if (result1.ResultTypeID != result2.ResultTypeID)
//                throw new Exception("ResultTypeID must be the same");

//            var resultType = new SportsAndResultTypesRepository().ResultTypes.SingleOrDefault(i => i.ResultTypeID == result1.ResultTypeID);
//            if (resultType == null)
//                throw new Exception("Unknown ResultTypeID=" + result1.ResultTypeID);

//            if (resultType.IsCountRequired == true)
//            {
//                double diff = result1.Count.Value - result2.Count.Value;
//                if (diff != 0)
//                    return (int)diff;
//                if (result1.Count2 != null && result2.Count2 != null)
//                    return (int)(result1.Count2.Value - result2.Count2.Value);
//                return 0;
//            }
//            else
//            {
//                double diff = 10000 * (-result1.Time.Value + result2.Time.Value);
//                if (diff > 0 && diff < 1)
//                    diff = 1;
//                if (diff < 0 && diff > -1)
//                    diff = -1;
//                return (int)diff;
//            }
//        }

//        public bool IsBest(Result result, List<Result> results)
//        {
//            foreach (var r in results)
//                if (Compare(result, r) <= 0)
//                    return false;
//            return true;
//        }
//    }
//}

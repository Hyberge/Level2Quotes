using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes.Task
{
    class LinearRegressionTrainingTask: ITask
    {
        public List<DataMining.TrainingExample> TrainingSet = new List<DataMining.TrainingExample>();

        public LinearRegressionTrainingTask(ITask Next): base(Next)
        { }

        public override bool TransactionProcessing()
        {
            bool ret = false;


            return ret;
        }
    }
}

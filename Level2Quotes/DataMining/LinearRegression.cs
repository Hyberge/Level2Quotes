using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes.DataMining
{
    class LinearRegression
    {
        int mFeatrueNumber = 0;
        double mJTheta;
        double mAlpha;
        double[] mParameterList = null;

        public LinearRegression(int FeatrueNum, double Alpha)
        {
            mAlpha = Alpha;

            mFeatrueNumber = FeatrueNum;
            mParameterList = new double[FeatrueNum + 1];

            for (int i=0; i<= FeatrueNum; ++i)
            {
                mParameterList[i] = 0;
            }
        }

        public void InitWithTrainingResults(int FeatrueNum, double[] Parameters)
        {
            mFeatrueNumber = FeatrueNum;
            mParameterList = Parameters;
        }

        private double GetHypothesis(double[] SampleData)
        {
            double Hypothesis = mParameterList[0];
            for (int i = 0; i < SampleData.Length; ++i)
            {
                Hypothesis += SampleData[i] * mParameterList[i + 1];
            }

            return Hypothesis;
        }

        public void Training(List<TrainingExample> TrainingSet)
        {
            int OscillationCount = 0;
            double[] HypothesisSet = new double[TrainingSet.Count];
            // 计算初值
            for (int i = 0; i < TrainingSet.Count; ++i)
            {
                HypothesisSet[i] = GetHypothesis(TrainingSet[i].SampleData);
                mJTheta += Math.Pow(HypothesisSet[i] - TrainingSet[i].Target, 2);
            }
            mJTheta *= 0.5;

            // 开始训练，收敛条件为在局部最优解处震荡5次
            while (OscillationCount < 5)
            {
                // 更新θ
                for (int i = 0; i < TrainingSet.Count; ++i)
                {
                    double Delta = (TrainingSet[i].Target - HypothesisSet[i]) * mAlpha;
                    mParameterList[0] += Delta;
                    for (int n = 1; i < mParameterList.Length; ++n)
                    {
                        mParameterList[n] += Delta * TrainingSet[i].SampleData[i];
                    }
                }

                // 重新计算J(θ)
                double JTheta = 0;
                for (int i = 0; i < TrainingSet.Count; ++i)
                {
                    HypothesisSet[i] = GetHypothesis(TrainingSet[i].SampleData);
                    JTheta += Math.Pow(HypothesisSet[i] - TrainingSet[i].Target, 2);
                }
                JTheta *= 0.5;

                // 判断是否趋近局部最优解，趋近则步调减半
                if (JTheta > mJTheta)
                {
                    mAlpha *= 0.5f;
                    OscillationCount++;
                }

                mJTheta = JTheta;
            }
        }

        /*
         * TrainingSample.Target作为返回结果
         */
        public void Predict(TrainingExample Example)
        {
            Example.Target = GetHypothesis(Example.SampleData);
        }
    }
}

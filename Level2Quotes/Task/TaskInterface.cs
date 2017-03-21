using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Level2Quotes.Task
{
    public enum TaskState
    {
        Unstarted,
        Running,
        Completion,
        Failed,
    }

    public abstract class ITask
    {
        ITask mNextTask;
        TaskState mState = TaskState.Unstarted;

        public delegate void DelegateMethod();

        DelegateMethod mCodeWhenCompleted = null;

        public ITask(ITask Next = null)
        {
            mNextTask = Next;
        }

        /*
         * 运行优先于NextTask
         */
        public void AddDelegateMethodWhenCompleted(DelegateMethod Code)
        {
            mCodeWhenCompleted = Code;
        }

        public TaskState GetState()
        {
            if (mState == TaskState.Completion && mNextTask != null)
            {
                return mNextTask.GetState();
            }
            else
            {
                return mState;
            }
        }

        public void DoTask()
        {
            mState = TaskState.Running;

            if (TransactionProcessing())
            {
                mState = TaskState.Completion;
                if (mCodeWhenCompleted != null)
                    mCodeWhenCompleted.Invoke();
                if (mNextTask != null)
                    mNextTask.DoTask();
            }
            else
            {
                mState = TaskState.Failed;
            }
        }

        public abstract bool TransactionProcessing();
    }
}

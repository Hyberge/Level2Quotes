using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;

namespace Level2Quotes.Task
{
    enum TaskSchedulingMode
    {
        TSM_StartNow = 0x1,
        TSM_StartAtTime = 0x2,
        TSM_EndAtTime = 0x4,
        TSM_EndWithTask = 0x8,
    }

    class TaskManager
    {
        struct TaskContext
        {
            public int StartTime;
            public int EndTime;

            public ITask Target;

            public Thread Executor;

            public TaskSchedulingMode Mode;
        }

        AtomLock mLock = new AtomLock();

        List<TaskContext> mTarget = new List<TaskContext>();

        Thread mSchedulerThread;

        volatile static TaskManager sInstance = null;

        public static TaskManager Instance() 
        {
            if (sInstance == null)
                sInstance = new TaskManager();
            return sInstance; 
        }

        private TaskManager()
        {
            Start();
        }

        private void Start()
        {
            mSchedulerThread = new Thread(o =>
            {
                while (true)
                {
                    mLock.Lock();

                    foreach (var ele in mTarget)
                    {
                        CheckTaskContext(ele);
                    }

                    mLock.Unlock();

                    Thread.Sleep(1000);
                }
            });

            mSchedulerThread.IsBackground = true;
            mSchedulerThread.Start();
        }

        public void HostedTask(ITask InTask, TaskSchedulingMode Mode, int StartTime = 0, int EndTime = 0)
        {
            TaskContext Context = new TaskContext();
            Context.Executor = new Thread(o => { Context.Target.DoTask(); });
            Context.Target = InTask;
            Context.Mode = Mode;
            Context.StartTime = StartTime;
            Context.EndTime = EndTime;

            mLock.Lock();

            mTarget.Add(Context);

            mLock.Unlock();
        }

        private void CheckTaskContext(TaskContext Context)
        {
            int NowTime = Util.DateTimeToTradingTime(DateTime.Now);

            switch (Context.Target.GetState())
            {
                case TaskState.Unstarted:
                    if (((Context.Mode & TaskSchedulingMode.TSM_StartAtTime) == TaskSchedulingMode.TSM_StartAtTime &&
                        NowTime >= Context.StartTime) ||
                        (Context.Mode & TaskSchedulingMode.TSM_StartNow) == TaskSchedulingMode.TSM_StartNow)
                        Context.Executor.Start();
                    break;
                case TaskState.Running:
                    if ((Context.Mode & TaskSchedulingMode.TSM_EndAtTime) == TaskSchedulingMode.TSM_EndAtTime &&
                        NowTime >= Context.EndTime)
                        Context.Target.TerminateTask();
                    break;
                default:
                    break;
            }
        }
    }
}

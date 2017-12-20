using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Enterprises.Framework.ThreadTasks
{
    public class ThreadPool
    {
        //线程锁对象  
        private static object lockObj = new object();
        //任务队列  
        private static Queue<ThreadStart> threadStartQueue = new Queue<ThreadStart>();
        //记录当前工作的任务集合，从中可以判断当前工作线程使用数，如果使用int判断的话可能会有问题，  
        //用集合的话还能取得对象的引用，比较好  
        private static HashSet<ThreadStart> threadsWorker = new HashSet<ThreadStart>();
        //当前允许最大工作线程数  
        private static int _maxThreadWorkerCount = 1;
        //当前允许最小工作线程数  
        private static int _minThreadWorkerCount = 0;

        /// <summary>  
        /// 设定最大工作线程数  
        /// </summary>  
        /// <param name="maxThreadCount">数量</param>  
        public static void SetMaxWorkThreadCount(int maxThreadCount)
        {
            _maxThreadWorkerCount = _minThreadWorkerCount > maxThreadCount
                ? _minThreadWorkerCount
                : maxThreadCount;
        }

        /// <summary>  
        /// 设定最小工作线程数  
        /// </summary>  
        /// <param name="minThreadCount">数量</param>  
        public static void SetMinWorkThreadCount(int minThreadCount)
        {
            _minThreadWorkerCount = minThreadCount > _maxThreadWorkerCount
                ? _maxThreadWorkerCount
                : minThreadCount;
        }

        /// <summary>  
        /// 启动线程池工作  
        /// </summary>  
        /// <param name="threadStartArray">任务数组</param>  
        public static void MyQueueUserWorkItem(List<ThreadStart> threadStartArray)
        {
            //将任务集合都放入到线程池中  
            AddAllThreadsToPool(threadStartArray);
            //线程池执行任务  
            ExcuteTask();
        }

        /// <summary>  
        /// 将单一任务加入队列中  
        /// </summary>  
        /// <param name="ts">单一任务对象</param>  
        private static void AddThreadToQueue(ThreadStart ts)
        {
            lock (lockObj)
            {
                threadStartQueue.Enqueue(ts);
            }
        }

        /// <summary>  
        /// 将多个任务加入到线程池的任务队列中  
        /// </summary>  
        /// <param name="threadStartArray">多个任务</param>  
        private static void AddAllThreadsToPool(List<ThreadStart> threadStartArray)
        {
            foreach (var threadStart in threadStartArray)
                AddThreadToQueue(threadStart);
        }

        /// <summary>  
        /// 执行任务，判断队列中的任务数量是否大于0，如果是则判断当前正在使用的工作线程的  
        /// 数量是否大于等于允许的最大工作线程数，如果一旦有线程空闲的话  
        /// 就会执行ExcuteTaskInQueen方法处理任务  
        /// </summary>  
        private static void ExcuteTask()
        {
            while (threadStartQueue.Count > 0)
            {
                System.Threading.Thread.Sleep(100);
                if (threadsWorker.Count < _maxThreadWorkerCount)
                {
                    ExcuteTaskInQueen();
                }
            }
        }

        /// <summary>  
        /// 执行出对列的任务，加锁保护  
        /// </summary>  
        private static void ExcuteTaskInQueen()
        {
            lock (lockObj)
            {
                ExcuteTaskByThread(
                    threadStartQueue.Dequeue());
            }
        }

        /// <summary>  
        /// 实现细节，这里使用BackGroudWork来实现后台线程  
        /// 注册doWork和Completed事件，当执行一个任务前，前将任务加入到  
        /// 工作任务集合（表示工作线程少了一个空闲），一旦RunWorkerCompleted事件被触发则将任务从工作  
        /// 任务集合中移除（表示工作线程也空闲了一个）  
        /// </summary>  
        /// <param name="threadStart"></param>  
        private static void ExcuteTaskByThread(ThreadStart threadStart)
        {
            threadsWorker.Add(threadStart);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, e) => { threadStart.Invoke(); };
            worker.RunWorkerCompleted += (o, e) => { threadsWorker.Remove(threadStart); };
            worker.RunWorkerAsync();
        }
    }
}

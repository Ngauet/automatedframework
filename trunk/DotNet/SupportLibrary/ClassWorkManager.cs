using System;
using System.Collections;
using System.Threading;
using System.Diagnostics;
namespace SupportLibrary
{
  /// <summary>
  /// Summary description for Class1.
  /// </summary>
  public class ClassWorkManager
  {
    private int ExpectedCompleteElements;
    private Queue WorkQueue;
    private Queue CompletedQueue ;
    public delegate void WorkFinished(ArrayList Results );
    public delegate object WorkToDo(object Param );
    private WorkFinished Notify;
    private ClassWorkUnit[] Workers;
    private ManualResetEvent[] ReadyForWork;
    private AutoResetEvent[] ResultsReady;
    private Thread Manager ;
    private ManualResetEvent WorkAvailable ;
    private  AutoResetEvent FinishedWithWork ;
    private ArrayList ResultObjects ;
    public class Unit
    {
      public delegate void UnitFinished(Unit Result);
      public Unit(ClassWorkManager.WorkToDo CurrentWork , object Param )
      {
        this.Finished=null;
        this.Param = Param;
        this.CurrentWork = CurrentWork;
      }
      public Unit(ClassWorkManager.WorkToDo CurrentWork , object Param ,UnitFinished Finished)
      {
        this.Finished=Finished;
        this.Param = Param;
        this.CurrentWork = CurrentWork;
      }
      private UnitFinished Finished;
      private object Param;
      private ClassWorkManager.WorkToDo CurrentWork;
      private object Result ;
      public object DoWork() 
      {
        Result = CurrentWork(Param);
        if (Finished != null)
        {
          Finished(this);
        }
        return Result;
      }

      public object Results
      {
        get
        {
          return Result;
        }
      }

      public object Parameter
      {
        get
        {
          return Param;
        }
      }
    }        

    public ClassWorkManager(long HowManyWorkers )
    {

        if (HowManyWorkers > 64)
        {
            throw new Exception("More than 64 threads is not allowed");
        }
      WorkQueue = Queue.Synchronized(new Queue());
      CompletedQueue = Queue.Synchronized(new Queue());
      WorkAvailable = new ManualResetEvent(false);
      FinishedWithWork = new AutoResetEvent(false);
      Manager = new Thread(new ThreadStart(ManagerMethod));
      Manager.Name = "Manager";
      Manager.IsBackground = true;
      Manager.Start();
      // WaitAny can wait on at most 64 WaitHandles
      Workers = new ClassWorkUnit[HowManyWorkers];
      ReadyForWork = new ManualResetEvent[HowManyWorkers];
      ResultsReady = new AutoResetEvent[HowManyWorkers];
      for (long i = 0;i<HowManyWorkers ;i++)
      {
        Workers[i] = new ClassWorkUnit("Worker" + i.ToString());
        ReadyForWork[i] = Workers[i].ReadyForWork;
        ResultsReady[i] = Workers[i].ResultsReady;
      }
    }
    public void DoWork(Unit[] Work, WorkFinished Finished )
    {
      if( Notify  != null)
      {
        throw new Exception("Work already in progress");
      }
      Notify = Finished;
      ExpectedCompleteElements=Work.Length ;
      CompletedQueue.Clear();
      for (long i = 0;i <Work.Length ;i++)
      {
        WorkQueue.Enqueue(Work[i]);
      }
      WorkAvailable.Set();
    }
    public bool[] WorkerThreadAvailability()
    {
      bool[] Results = new bool[Workers.Length ];
      for (long i = 0;i <Workers.Length ;i++)
      {
        Results[i]= ReadyForWork[i].WaitOne(0,false);
      }
      return Results;
    }
    private void ManagerMethod()
    {
      bool Signaled ;
      int ThreadReadyForWork;
      Unit WorkUnit ;
      while (true)
      {
        Signaled = WorkAvailable.WaitOne(100, false);
        if (Signaled) 
        {
          ThreadReadyForWork= WaitHandle.WaitAny(ReadyForWork,100,false);
          if (ThreadReadyForWork != WaitHandle.WaitTimeout)
          {
            WorkUnit =null;
            if (WorkQueue.Count > 0)
            {
              WorkUnit = (Unit) WorkQueue.Dequeue();
              if (WorkUnit == null)
              {
                throw new Exception("Work is null");
              }
              Workers[ThreadReadyForWork].Work(WorkUnit);
            }
          }
          ThreadReadyForWork= WaitHandle.WaitAny(ResultsReady,100,false);
          if (ThreadReadyForWork != WaitHandle.WaitTimeout)
          {
            WorkUnit = Workers[ThreadReadyForWork].GetResults();
            if (WorkUnit  != null)
            {
              CompletedQueue.Enqueue(WorkUnit);
            }
          }
          // All threads may have finsihed their work
          if (CompletedQueue.Count == ExpectedCompleteElements)
          {
            if (Notify != null) 
            {
              // Gather up the results and send them back
              ResultObjects = new ArrayList();
              while (CompletedQueue.Count > 0)
              {
                ResultObjects.Add(CompletedQueue.Dequeue());
              }
              Notify(ResultObjects);
              Notify = null;
            }
            WorkAvailable.Reset();
            FinishedWithWork.Set();
          }
        }
      }
    }

    public AutoResetEvent Finished
    {
      get
      {
        return FinishedWithWork;
      }
    }

    public ArrayList Results
    {
      get
      {
        return ResultObjects;
      }
    }

  }
}

using System;
using System.Threading;
using System.Collections;

namespace SupportLibrary
{
  /// <summary>
  /// 
  /// </summary>
  public class ClassWorkUnit
  {
    private Thread WorkThread ;
    private ManualResetEvent ReadyForWorkEvent ;
    private AutoResetEvent ResultsReadyEvent;
    private AutoResetEvent WorkAvailable ;
    private ClassWorkManager.Unit WorkUnit;
    public ClassWorkUnit(string Name)
    {
      ResultsReadyEvent = new AutoResetEvent(false);
      ReadyForWorkEvent = new ManualResetEvent(true); // initially signaled
      WorkAvailable = new AutoResetEvent(false);
      WorkThread = new Thread(new ThreadStart(ThreadMethod));
      WorkThread.IsBackground = true;
      WorkThread.Name = Name;
      WorkThread.Start();
    }
    public void Work(ClassWorkManager.Unit WorkUnit )
    {
      ReadyForWorkEvent.Reset();
      if (WorkUnit == null)
      {
        throw new Exception("Null work unit passed in");
      }
      this.WorkUnit = WorkUnit;
      WorkAvailable.Set();
    }
    private void ThreadMethod()
    {
      while (true)
      {
        WorkAvailable.WaitOne();
        if (WorkUnit == null)
        {
          throw new Exception("Work is null");
        }
        WorkUnit.DoWork();
        ResultsReadyEvent.Set();
      }
    }

    public ManualResetEvent ReadyForWork
    {
      get
      {
        return ReadyForWorkEvent;
      }
    }
    public AutoResetEvent ResultsReady
    {
      get
      {
        return ResultsReadyEvent;
      }
    }
    public ClassWorkManager.Unit GetResults() 
    {
      ClassWorkManager.Unit TempWorkUnit;
      TempWorkUnit=this.WorkUnit;
      this.WorkUnit = null;
      ReadyForWorkEvent.Set();
      return TempWorkUnit ;
    }
  }
}

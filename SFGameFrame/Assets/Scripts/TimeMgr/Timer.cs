using UnityEngine;

  public class Timer
    {
        public Timer(float duration)
        {
            this.duration = duration;
        }
        private float startTime { get; set; }
        private float endTime { get; set; }
        private float duration { get; set; }
        private float curTime { get; set; }
        private bool isStart { get; set; }
        private bool isPause { get; set; }

        public delegate void CallBack(float duration);

        public CallBack OnStart { get; set; }
        public CallBack OnEnd { get; set; }
        public CallBack OnUpdate { get; set; }
        public CallBack OnCanel { get; set; }

        public void Start()
        {
            isStart = true;
            startTime = Time.time;
            curTime = startTime;
            endTime = curTime + duration;
            TimeMgr.AddTimer(this);
            if (OnStart != null)
                OnStart(duration);
        }

        public void Update()
        {
            if (!isStart) return;
            curTime += Time.deltaTime;
            if(isPause)
            {
                endTime += Time.deltaTime;
            }
            if (curTime > endTime)
            {
                end();
            }
            else if (OnUpdate != null && !isPause)
            OnUpdate(this.duration);
        }
        public void Canel()
        {
            isStart = false;
            if (OnCanel != null)
                OnCanel(duration);
        }

	    public void SetDuration(float duration)
	    {
            this.duration = duration;
	    }

	    public float GetDuration()
	    {
            return this.duration;
	    }

        public void Pause()
        {
            isPause = true;
        }

        public void Continue()
        {
            isPause = false;
        }

	    public bool IsPause()
	    {
	        return isPause;
	    }

        public float IsStart()
        {
            if (curTime >= endTime)
                return -1;
            return endTime - curTime;
        }

        private void end()
        {
            isStart = false;
            if (OnEnd != null)
                OnEnd(duration);
            TimeMgr.RemoveTimer(this);
        }
    }


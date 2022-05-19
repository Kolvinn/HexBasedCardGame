using Godot;
public class ReferenceTimer : Timer
    {
        [Signal]
        public delegate void ReferenceTimerTimeout(ReferenceTimer t);
        public ReferenceTimer(){
            this.Connect("timeout", this, nameof(On_Timeout));
        }

        public void On_Timeout()
        {
            EmitSignal("ReferenceTimerTimeout", this);
        }
    }
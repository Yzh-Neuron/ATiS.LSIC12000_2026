using System;

namespace ATiS.Sim.Wpf
{
    public class GrowlInfo
    {
        public string? Message { get; set; }
        public MessageType Type { get; set; }
        public bool ShowDateTime { get; set; } = true;
        public DateTime Time { get; set; } = DateTime.Now;
        public bool ShowCloseButton { get; set; } = true;
        public bool StayOpen { get; set; } = false;
        public int WaitSeconds { get; set; } = 5;
        public Action<bool>? CallbackAfterClose { get; set; }
        public string? IconKey { get; set; }
        public string? IconBrushKey { get; set; }
        public bool IsPanelDockLeft { get; set; }
    }

}

namespace LSIC.App.Models
{
    public class Controller : TcpHangKeClient<ControllerFunctionCode>
    {
        private bool? _doorOpened;
        public bool? DoorOpened
        {
            get => _doorOpened;
            set => SetProperty(ref _doorOpened, value);
        }
        private LightAlarmState? _lightAlarm;
        public LightAlarmState? LightAlarm
        {
            get => _lightAlarm;
            set => SetProperty(ref _lightAlarm, value);
        }
        private WaterCoolerState? _chillerState;
        public WaterCoolerState? ChillerState
        {
            get => _chillerState;
            set => SetProperty(ref _chillerState, value);
        }
        public Controller(ControllerInfo info) : base(info.Id, info.Ip, info.Port)
        {
        }
        protected override ValueTask OnAnalysisFrameAsync(ControllerFunctionCode funcCode, byte[] data)
        {
            int index = 0;
            switch (funcCode)
            {
                case ControllerFunctionCode.ChillerFeedback:
                    {
                        ChillerState = (WaterCoolerState)data[index++];
                        byte count = data[index++];
                        var zones = Device.Instance.Zones;

                        if (zones != null)
                        {
                            for (byte i = 0; i < count; i++)
                            {
                                int flowRate = data.ToInt32(index, Endianness.Big);
                                index += 4;

                                var pipe = zones.FirstOrDefault(z => z.Id == i)?.WaterCoolingPipe;
                                if (pipe != null)
                                {
                                    pipe.FlowRate = flowRate / 10.0d;
                                }
                            }
                        }
                        break;
                    }
                case ControllerFunctionCode.WaterValveControlFeedback:
                    {
                        byte count = data[index++];
                        var zones = Device.Instance.Zones;

                        if (zones != null)
                        {
                            for (byte i = 0; i < count; i++)
                            {
                                int valFeedback = data.ToInt32(index, Endianness.Big);
                                index += 4;

                                var pipe = zones.FirstOrDefault(z => z.Id == i)?.WaterCoolingPipe;
                                if (pipe != null)
                                {
                                    pipe.ValveFeedback = valFeedback / 10.0d;
                                }
                            }
                        }
                        break;
                    }
                case ControllerFunctionCode.AgingDoorControlFeedback:
                    {
                        DoorOpened = data[index] switch
                        {
                            0x00 => false,
                            0x01 => true,
                            _ => null
                        };
                        break;
                    }
                case ControllerFunctionCode.AlarmLightControlFeedback:
                    {
                        LightAlarmState set = (LightAlarmState)data[index];
                        LightAlarm = set;
                        break;
                    }
            }

            return ValueTask.CompletedTask;
        }
        protected override ValueTask OnConnectedAsync()
        {
            return ValueTask.CompletedTask;
        }
        protected override ValueTask OnDisconnectedAsync()
        {
            ChillerState = null;
            DoorOpened = null;
            LightAlarm = null;
            return ValueTask.CompletedTask;
        }

        #region 控制板功能
        /// <summary>
        /// 水阀控制
        /// </summary>
        public async ValueTask SendWaterValveControlAsync(byte id, uint val)
        {
            var data = new byte[5];
            data[0] = id;
            Array.Copy(val.ToBytes(Endianness.Big), 0, data, 1, 4);
            await SendAsync(ControllerFunctionCode.WaterValveControlFeedback, data);
        }

        /// <summary>
        /// 老化门控制
        /// </summary>
        /// <param name="open"></param>
        /// <returns></returns>
        public async ValueTask SendAgingDoorControlAsync(bool open)
        {
            var data = new[] { open ? (byte)0x01 : (byte)0x00 };
            await SendAsync(ControllerFunctionCode.AgingDoorControlFeedback, data);
        }

        /// <summary>
        /// 报警灯控制
        /// </summary>
        public async ValueTask SendAlarmLightControlAsync(LightAlarmState state)
        {
            var data = new[] { (byte)state };
            await SendAsync(ControllerFunctionCode.AlarmLightControlFeedback, data);
        }
        #endregion
    }
}
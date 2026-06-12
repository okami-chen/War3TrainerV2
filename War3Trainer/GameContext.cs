using System;
using System.Diagnostics;

namespace War3Trainer
{
    class GameContext
    {
        public int ProcessId                  { get; private set; }
        public string ProcessVersion          { get; private set; }

        public UInt32 ThisGameAddress         { get; private set; }
        public UInt32 UnitListAddress         { get; private set; }
        public UInt32 MoveSpeedAddress        { get; private set; }
        public UInt32 UnitAddAbilityAddress   { get; private set; }
        public UInt32 UnitRemoveAbilityAddress { get; private set; }
        public UInt32 UnitSetAbilityLevelAddress { get; private set; }
        public UInt32 UnitFindAbilityAddress { get; private set; }
        public UInt32 UnitRefreshAbilityAddress { get; private set; }
        public UInt32 UnitBeginAbilityUpdateAddress { get; private set; }
        public UInt32 UnitEndAbilityUpdateAddress { get; private set; }
        public UInt32 UnitGetAbilityMaxLevelAddress { get; private set; }
        public UInt32 JassStateGlobalAddress { get; private set; }
        public UInt32 JassGetManagerAddress { get; private set; }
        public UInt32 JassGetAgentByObjectAddress { get; private set; }
        public UInt32 JassHandleToUnitAddress { get; private set; }
        public UInt32 JassUnitAddAbilityAddress { get; private set; }
        public UInt32 JassUnitRemoveAbilityAddress { get; private set; }
        public UInt32 JassSetUnitAbilityLevelAddress { get; private set; }
        public UInt32 JassGetOwningPlayerAddress { get; private set; }
        public UInt32 JassGetPlayerIdAddress { get; private set; }
        public UInt32 JassKillUnitAddress { get; private set; }

        public UInt32 AttackAttributesOffset  { get; private set; }
        public UInt32 HeroAttributesOffset    { get; private set; }
        public UInt32 ItemsListOffset         { get; private set; }
        public UInt32 MoveSpeedOffset         { get; private set; }

        private uint _moduleAddress;

        // Get a context if the game is running and recognized.
        // Returns null if not running.
        // Raise exception if game version is not recognized.
        public static GameContext FindGameRunning(string processName, string moduleName)
        {
            Process[] processesByName = Process.GetProcessesByName(processName);
            if (processesByName.Length > 0)
            {
                return new GameContext(processesByName[0], moduleName);
            }
            return null;
        }

        public GameContext(Process gameProcess, string moduleName)
        {
            // Get PID
            GetProcessInfo(gameProcess);

            // Find module
            GetModuleInfo(gameProcess, moduleName);

            // Decide addresses according to version
            GetGameAddress();

            // Decide ability function addresses according to version
            GetAbilityFunctionAddress();

            // Decide offsets according to version
            GetGameOffset();
        }

        private void GetProcessInfo(Process gameProcess)
        {
            try
            {
                this.ProcessId = gameProcess.Id;
            }
            catch
            {
                throw new InvalidOperationException("Failed to fetch process Id");
            }
        }

        private void GetModuleInfo(Process gameProcess, string moduleName)
        {
            // Find module
            WindowsApi.ProcessModule mainModule =
                new WindowsApi.ProcessModule(
                    ProcessId,
                    moduleName);

            // Check parameters
            string moduleFileName = mainModule.FileName;
            System.Diagnostics.FileVersionInfo moduleVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(moduleFileName);
            string fileVersion = moduleVersion.FileVersion;
            if (fileVersion == null)
                throw new InvalidOperationException("Bad file version");

            // Save info
            this.ProcessVersion = fileVersion.Replace(", ", ".");
            _moduleAddress = (uint)mainModule.BaseAddress;
        }

        private void GetGameAddress()
        {
            switch (ProcessVersion)
            {
                case "1.20.4.6074":
                    ThisGameAddress  = _moduleAddress + 0x87C744;
                    UnitListAddress  = _moduleAddress + 0x8722BC;
                    MoveSpeedAddress = _moduleAddress + 0x55BDF0;
                    break;
                case "1.21.0.6263":
                    ThisGameAddress  = _moduleAddress + 0x87D7BC;
                    UnitListAddress  = _moduleAddress + 0x873334;
                    MoveSpeedAddress = _moduleAddress + 0x55FE80;
                    break;
                case "1.21.1.6300":
                    ThisGameAddress  = _moduleAddress + 0x87D7BC;
                    UnitListAddress  = _moduleAddress + 0x873334;
                    MoveSpeedAddress = _moduleAddress + 0x55fEA0;
                    break;
                case "1.22.0.6328":
                    ThisGameAddress  = _moduleAddress + 0xAA4178;
                    UnitListAddress  = _moduleAddress + 0xAA2FFC;
                    MoveSpeedAddress = _moduleAddress + 0x201190;
                    break;
                case "1.23.0.6352":
                    ThisGameAddress  = _moduleAddress + 0xABCFC8;
                    UnitListAddress  = _moduleAddress + 0xABBE4C;
                    MoveSpeedAddress = _moduleAddress + 0x2026D0;
                    break;
                case "1.24.0.6372":
                case "1.24.1.6374":
                case "1.24.2.6378":
                case "1.24.3.6384":
                    ThisGameAddress  = _moduleAddress + 0xACE5E0;
                    UnitListAddress  = _moduleAddress + 0xACD44C;
                    MoveSpeedAddress = _moduleAddress + 0x202780;
                    break;
                case "1.24.4.6387":
                    ThisGameAddress  = _moduleAddress + 0xACE5E0;
                    UnitListAddress  = _moduleAddress + 0xACD44C;
                    MoveSpeedAddress = _moduleAddress + 0x2027E0;
                    break;
                case "1.25.1.6397":
                    ThisGameAddress  = _moduleAddress + 0xAB7788;
                    UnitListAddress  = _moduleAddress + 0xAB65F4;
                    MoveSpeedAddress = _moduleAddress + 0x201AA0;
                    break;
                case "1.26.0.6401":
                    ThisGameAddress  = _moduleAddress + 0xAB7788;
                    UnitListAddress  = _moduleAddress + 0xAB65F4;
                    MoveSpeedAddress = _moduleAddress + 0x201CD0;
                    break;
                case "1.27.0.52240":
                    ThisGameAddress  = _moduleAddress + 0xBE40A8;
                    UnitListAddress  = _moduleAddress + 0xBE4238;
                    MoveSpeedAddress = _moduleAddress + 0x5DF420;
                    break;
                case "1.28.0.7205":
                    ThisGameAddress = _moduleAddress + 0xD72F58;
                    UnitListAddress = _moduleAddress + 0xD730F0;
                    MoveSpeedAddress = _moduleAddress + 0x604470;
                    break;
                case "1.28.5.7680":
                    ThisGameAddress = _moduleAddress + 0xD30448;
                    UnitListAddress = _moduleAddress + 0xD305E0;
                    MoveSpeedAddress = _moduleAddress + 0x630C70;
                    break;
                default:
                    throw new UnkonwnGameVersionExpection(
                        this.ProcessId,
                        ProcessVersion);
            }
        }

        private void GetAbilityFunctionAddress()
        {
            UnitAddAbilityAddress = 0;
            UnitRemoveAbilityAddress = 0;
            UnitSetAbilityLevelAddress = 0;
            UnitFindAbilityAddress = 0;
            UnitRefreshAbilityAddress = 0;
            UnitBeginAbilityUpdateAddress = 0;
            UnitEndAbilityUpdateAddress = 0;
            UnitGetAbilityMaxLevelAddress = 0;
            JassStateGlobalAddress = 0;
            JassGetManagerAddress = 0;
            JassGetAgentByObjectAddress = 0;
            JassHandleToUnitAddress = 0;
            JassUnitAddAbilityAddress = 0;
            JassUnitRemoveAbilityAddress = 0;
            JassSetUnitAbilityLevelAddress = 0;
            JassGetOwningPlayerAddress = 0;
            JassGetPlayerIdAddress = 0;
            JassKillUnitAddress = 0;

            switch (ProcessVersion)
            {
                case "1.20.4.6074":
                    UnitAddAbilityAddress = _moduleAddress + 0x5CC280;
                    UnitRemoveAbilityAddress = _moduleAddress + 0x486770;
                    UnitSetAbilityLevelAddress = _moduleAddress + 0x2C4450;
                    UnitFindAbilityAddress = _moduleAddress + 0x4865B0;
                    UnitRefreshAbilityAddress = _moduleAddress + 0x124CA0;
                    UnitBeginAbilityUpdateAddress = _moduleAddress + 0x1E49B0;
                    UnitEndAbilityUpdateAddress = _moduleAddress + 0x1E4910;
                    UnitGetAbilityMaxLevelAddress = _moduleAddress + 0x43BCF0;
                    JassStateGlobalAddress = _moduleAddress + 0x8722BC;
                    JassGetManagerAddress = _moduleAddress + 0x29B2E0;
                    JassGetAgentByObjectAddress = _moduleAddress + 0x327570;
                    JassHandleToUnitAddress = 0;
                    JassUnitAddAbilityAddress = _moduleAddress + 0x2C84E0;
                    JassUnitRemoveAbilityAddress = _moduleAddress + 0x2C8610;
                    JassSetUnitAbilityLevelAddress = _moduleAddress + 0x2C4450;
                    break;
                case "1.21.0.6263":
                    UnitAddAbilityAddress = _moduleAddress + 0x5D02A0;
                    UnitRemoveAbilityAddress = _moduleAddress + 0x48A6A0;
                    UnitSetAbilityLevelAddress = _moduleAddress + 0x2C7A20;
                    UnitFindAbilityAddress = _moduleAddress + 0x48A4E0;
                    UnitRefreshAbilityAddress = _moduleAddress + 0x124CC0;
                    UnitBeginAbilityUpdateAddress = _moduleAddress + 0x1E7B70;
                    UnitEndAbilityUpdateAddress = _moduleAddress + 0x1E7AD0;
                    UnitGetAbilityMaxLevelAddress = _moduleAddress + 0x43FC20;
                    JassStateGlobalAddress = _moduleAddress + 0x873334;
                    JassGetManagerAddress = _moduleAddress + 0x29E5C0;
                    JassGetAgentByObjectAddress = _moduleAddress + 0x32B700;
                    JassHandleToUnitAddress = 0;
                    JassUnitAddAbilityAddress = _moduleAddress + 0x2CBAB0;
                    JassUnitRemoveAbilityAddress = _moduleAddress + 0x2CBBE0;
                    JassSetUnitAbilityLevelAddress = _moduleAddress + 0x2C7A20;
                    break;
                case "1.21.1.6300":
                case "1.24.0.6372":
                case "1.24.2.6378":
                case "1.24.3.6384":
                case "1.28.0.7205":
                case "1.28.5.7680":
                    // Fill these with addresses for the matching version.
                    // If you have a game.dll offset, use: _moduleAddress + 0xOFFSET.
                    // If you have a runtime absolute address, assign it directly.
                    break;
                case "1.22.0.6328":
                    UnitAddAbilityAddress = _moduleAddress + 0x24C2B0;
                    UnitRemoveAbilityAddress = _moduleAddress + 0x0792F0;
                    UnitSetAbilityLevelAddress = _moduleAddress + 0x3C0450;
                    UnitFindAbilityAddress = _moduleAddress + 0x077D80;
                    UnitRefreshAbilityAddress = _moduleAddress + 0x331CB0;
                    UnitBeginAbilityUpdateAddress = _moduleAddress + 0x2B2930;
                    UnitEndAbilityUpdateAddress = _moduleAddress + 0x2B2900;
                    UnitGetAbilityMaxLevelAddress = _moduleAddress + 0x02DCE0;
                    JassStateGlobalAddress = _moduleAddress + 0xAA2FFC;
                    JassGetManagerAddress = _moduleAddress + 0x3A6C50;
                    JassHandleToUnitAddress = _moduleAddress + 0x3B81D0;
                    JassUnitAddAbilityAddress = _moduleAddress + 0x3C1540;
                    JassUnitRemoveAbilityAddress = _moduleAddress + 0x3C15B0;
                    JassSetUnitAbilityLevelAddress = _moduleAddress + 0x3C0450;
                    break;
                case "1.23.0.6352":
                    UnitAddAbilityAddress = _moduleAddress + 0x24D7F0;
                    UnitRemoveAbilityAddress = _moduleAddress + 0x07A830;
                    UnitSetAbilityLevelAddress = _moduleAddress + 0x3C2170;
                    UnitFindAbilityAddress = _moduleAddress + 0x079340;
                    UnitRefreshAbilityAddress = _moduleAddress + 0x3331F0;
                    UnitBeginAbilityUpdateAddress = _moduleAddress + 0x2B3E70;
                    UnitEndAbilityUpdateAddress = _moduleAddress + 0x2B3E40;
                    UnitGetAbilityMaxLevelAddress = _moduleAddress + 0x02F220;
                    JassStateGlobalAddress = _moduleAddress + 0xABBE4C;
                    JassGetManagerAddress = _moduleAddress + 0x3A8640;
                    JassHandleToUnitAddress = _moduleAddress + 0x3B9EF0;
                    JassUnitAddAbilityAddress = _moduleAddress + 0x3C3260;
                    JassUnitRemoveAbilityAddress = _moduleAddress + 0x3C32D0;
                    JassSetUnitAbilityLevelAddress = _moduleAddress + 0x3C2170;
                    break;
                case "1.24.1.6374":
                    UnitAddAbilityAddress = _moduleAddress + 0x24D8A0;
                    UnitRemoveAbilityAddress = _moduleAddress + 0x07A8E0;
                    UnitSetAbilityLevelAddress = _moduleAddress + 0x3C7C30;
                    UnitFindAbilityAddress = _moduleAddress + 0x0793F0;
                    UnitRefreshAbilityAddress = _moduleAddress + 0x3332B0;
                    UnitBeginAbilityUpdateAddress = _moduleAddress + 0x2B3F30;
                    UnitEndAbilityUpdateAddress = _moduleAddress + 0x2B3F00;
                    UnitGetAbilityMaxLevelAddress = _moduleAddress + 0x02F2D0;
                    JassStateGlobalAddress = _moduleAddress + 0xACD44C;
                    JassGetManagerAddress = _moduleAddress + 0x3A8AE0;
                    JassHandleToUnitAddress = _moduleAddress + 0x3BE730;
                    JassUnitAddAbilityAddress = _moduleAddress + 0x3C8D20;
                    JassUnitRemoveAbilityAddress = _moduleAddress + 0x3C8D90;
                    JassSetUnitAbilityLevelAddress = _moduleAddress + 0x3C7C30;
                    break;
                case "1.24.4.6387":
                    UnitAddAbilityAddress = _moduleAddress + 0x24D900;
                    UnitRemoveAbilityAddress = _moduleAddress + 0x07A8F0;
                    UnitSetAbilityLevelAddress = _moduleAddress + 0x3C7CF0;
                    UnitFindAbilityAddress = _moduleAddress + 0x079400;
                    UnitRefreshAbilityAddress = _moduleAddress + 0x333370;
                    UnitBeginAbilityUpdateAddress = _moduleAddress + 0x2B3FF0;
                    UnitEndAbilityUpdateAddress = _moduleAddress + 0x2B3FC0;
                    UnitGetAbilityMaxLevelAddress = _moduleAddress + 0x02F2B0;
                    JassStateGlobalAddress = _moduleAddress + 0xACD44C;
                    JassGetManagerAddress = _moduleAddress + 0x3A8BA0;
                    JassHandleToUnitAddress = _moduleAddress + 0x3BE7F0;
                    JassUnitAddAbilityAddress = _moduleAddress + 0x3C8DE0;
                    JassUnitRemoveAbilityAddress = _moduleAddress + 0x3C8E50;
                    JassSetUnitAbilityLevelAddress = _moduleAddress + 0x3C7CF0;
                    break;
                case "1.25.1.6397":
                    UnitAddAbilityAddress = _moduleAddress + 0x24CBC0;
                    UnitRemoveAbilityAddress = _moduleAddress + 0x079CC0;
                    UnitSetAbilityLevelAddress = _moduleAddress + 0x3C6F80;
                    UnitFindAbilityAddress = _moduleAddress + 0x0787D0;
                    UnitRefreshAbilityAddress = _moduleAddress + 0x332600;
                    UnitBeginAbilityUpdateAddress = _moduleAddress + 0x2B32A0;
                    UnitEndAbilityUpdateAddress = _moduleAddress + 0x2B3270;
                    UnitGetAbilityMaxLevelAddress = _moduleAddress + 0x02E570;
                    JassStateGlobalAddress = _moduleAddress + 0xAB65F4;
                    JassGetManagerAddress = _moduleAddress + 0x3A7E30;
                    JassHandleToUnitAddress = _moduleAddress + 0x3BDA80;
                    JassUnitAddAbilityAddress = _moduleAddress + 0x3C8070;
                    JassUnitRemoveAbilityAddress = _moduleAddress + 0x3C80E0;
                    JassSetUnitAbilityLevelAddress = _moduleAddress + 0x3C6F80;
                    break;
                case "1.26.0.6401":
                    UnitAddAbilityAddress = _moduleAddress + 0x24CDF0;
                    UnitRemoveAbilityAddress = _moduleAddress + 0x079CC0;
                    UnitSetAbilityLevelAddress = _moduleAddress + 0x3C71B0;
                    UnitFindAbilityAddress = _moduleAddress + 0x0787D0;
                    UnitRefreshAbilityAddress = _moduleAddress + 0x332830;
                    UnitBeginAbilityUpdateAddress = _moduleAddress + 0x2B34D0;
                    UnitEndAbilityUpdateAddress = _moduleAddress + 0x2B34A0;
                    UnitGetAbilityMaxLevelAddress = _moduleAddress + 0x02E570;
                    JassStateGlobalAddress = _moduleAddress + 0xAB65F4;
                    JassGetManagerAddress = _moduleAddress + 0x3A8060;
                    JassHandleToUnitAddress = _moduleAddress + 0x3BDCB0;
                    JassUnitAddAbilityAddress = _moduleAddress + 0x3C82A0;
                    JassUnitRemoveAbilityAddress = _moduleAddress + 0x3C8310;
                    JassSetUnitAbilityLevelAddress = _moduleAddress + 0x3C71B0;
                    break;
                case "1.27.0.52240":
                    UnitAddAbilityAddress = _moduleAddress + 0x454EB0;
                    UnitRemoveAbilityAddress = _moduleAddress + 0x471160;
                    UnitSetAbilityLevelAddress = _moduleAddress + 0x1F7140;
                    UnitFindAbilityAddress = _moduleAddress + 0x46F440;
                    UnitRefreshAbilityAddress = _moduleAddress + 0x3598C0;
                    UnitBeginAbilityUpdateAddress = _moduleAddress + 0x6AD4E0;
                    UnitEndAbilityUpdateAddress = _moduleAddress + 0x6AA8E0;
                    UnitGetAbilityMaxLevelAddress = _moduleAddress + 0x3F86C0;
                    JassStateGlobalAddress = _moduleAddress + 0xBE4238;
                    JassGetManagerAddress = _moduleAddress + 0x1C3200;
                    JassHandleToUnitAddress = _moduleAddress + 0x1D1550;
                    JassUnitAddAbilityAddress = _moduleAddress + 0x1FA510;
                    JassUnitRemoveAbilityAddress = _moduleAddress + 0x1FB0B0;
                    JassSetUnitAbilityLevelAddress = _moduleAddress + 0x1F7140;
                    JassGetOwningPlayerAddress = _moduleAddress + 0x1E3BA0;
                    JassGetPlayerIdAddress = _moduleAddress + 0x1E3D20;
                    JassKillUnitAddress = _moduleAddress + 0x1EFCD0;
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false, "Impossible to run to here");
                    break;
            }
        }

        private void GetGameOffset()
        {
            switch (ProcessVersion)
            {
                case "1.20.4.6074":
                case "1.21.0.6263":
                case "1.21.1.6300":
                case "1.22.0.6328":
                case "1.23.0.6352":
                case "1.24.0.6372":
                case "1.24.1.6374":
                case "1.24.2.6378":
                    AttackAttributesOffset = 0x1E4;
                    HeroAttributesOffset   = 0x1EC;
                    ItemsListOffset        = 0x1F4;
                    MoveSpeedOffset        = 0x1D8;
                    break;
                case "1.24.3.6384":
                case "1.24.4.6387":
                case "1.25.1.6397":
                case "1.26.0.6401":
                case "1.27.0.52240":
                case "1.28.0.7205":
                case "1.28.5.7680":
                    AttackAttributesOffset = 0x1E8;
                    HeroAttributesOffset   = 0x1F0;
                    ItemsListOffset        = 0x1F8;
                    MoveSpeedOffset        = 0x1DC;
                    break;
                default:
                    System.Diagnostics.Debug.Assert(false, "Impossible to run to here");
                    break;
            }
        }
    }
}

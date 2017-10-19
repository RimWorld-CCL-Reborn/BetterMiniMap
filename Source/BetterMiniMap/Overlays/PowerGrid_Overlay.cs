using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class PowerGrid_Overlay : Overlay, IExposable
	{
        public PowerGrid_Overlay(bool visible = true) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.updatePeriods.powerGrid;

		public override void Render()
		{
            foreach (Building poweredBuilding in Find.VisibleMap.listerBuildings.allBuildingsColonist.Where(b => b.PowerComp != null))
				this.DrawConnection(poweredBuilding.PowerComp);
        }

		private void DrawBattery(CompPowerBattery battery)
		{
			Color color = BetterMiniMapMod.settings.overlayColors.notPowered;
            if (battery.PowerNet?.CurrentEnergyGainRate() > 1f)
                color = BetterMiniMapMod.settings.overlayColors.poweredOn;
            else if (battery.StoredEnergy > 1f)
                color = BetterMiniMapMod.settings.overlayColors.poweredByBatteries;

            Utilities.DrawThing(base.Texture, battery.parent, color);
		}

		private void DrawConnection(CompPower powerComp)
		{
			if (powerComp is CompPowerTransmitter compPowerTransmitter)
				this.DrawTransmitter(compPowerTransmitter);

			if (powerComp is CompPowerTrader compPowerTrader)
				this.DrawTrader(compPowerTrader);

			if (powerComp is CompPowerBattery compPowerBattery)
				this.DrawBattery(compPowerBattery);
		}

		private void DrawTrader(CompPowerTrader trader)
		{
            Color color = BetterMiniMapMod.settings.overlayColors.powererOff;
            if (trader.PowerOn)
				color = BetterMiniMapMod.settings.overlayColors.poweredOn;
            else if (trader.PowerOutput == 0f)
                color = BetterMiniMapMod.settings.overlayColors.notPowered;

            Utilities.DrawThing(base.Texture, trader.parent, color);
		}

		private void DrawTransmitter(CompPowerTransmitter transmitter)
		{
			Color color = BetterMiniMapMod.settings.overlayColors.notPowered;
            if (transmitter.transNet != null)
			{
                if (transmitter.transNet.CurrentEnergyGainRate() > 0f)
                    color = BetterMiniMapMod.settings.overlayColors.poweredOn;
                else if (transmitter.transNet.CurrentStoredEnergy() > 1f)
                    color = BetterMiniMapMod.settings.overlayColors.poweredByBatteries;
            }
			Utilities.DrawThing(base.Texture, transmitter.parent, color);
		}

        public void ExposeData() => this.ExposeData("overlayPower");
	}
}

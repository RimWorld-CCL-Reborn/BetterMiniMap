using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
    // TODO: can this be abstracted into a def?
	public class PowerGridOverlay : Overlay, IExposable
	{
        public PowerGridOverlay(Map map, bool visible = true) : base(map, visible) { }

        public void ExposeData() => this.ExposeData("overlayPower");

		public override int GetUpdateInterval() => BetterMiniMapMod.modSettings.updatePeriods.powerGrid;

		public override void Render()
		{
            foreach (Building poweredBuilding in this.map.listerBuildings.allBuildingsColonist.Where(b => b.PowerComp != null))
				this.DrawConnection(poweredBuilding.PowerComp);
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

        private void DrawTransmitter(CompPowerTransmitter transmitter)
        {
            Color color = BetterMiniMapMod.modSettings.overlayColors.notPowered;
            if (transmitter.transNet != null)
            {
                if (transmitter.transNet.CurrentEnergyGainRate() > 0f)
                    color = BetterMiniMapMod.modSettings.overlayColors.poweredOn;
                else if (transmitter.transNet.CurrentStoredEnergy() > 1f)
                    color = BetterMiniMapMod.modSettings.overlayColors.poweredByBatteries;
            }
            this.DrawThing(transmitter.parent, color);
        }

        private void DrawTrader(CompPowerTrader trader)
		{
            Color color = BetterMiniMapMod.modSettings.overlayColors.powererOff;
            if (trader.PowerOn)
				color = BetterMiniMapMod.modSettings.overlayColors.poweredOn;
            else if (trader.PowerOutput == 0f)
                color = BetterMiniMapMod.modSettings.overlayColors.notPowered;

            this.DrawThing(trader.parent, color);
		}

        private void DrawBattery(CompPowerBattery battery)
        {
            Color color = BetterMiniMapMod.modSettings.overlayColors.notPowered;
            if (battery.PowerNet?.CurrentEnergyGainRate() > 1f)
                color = BetterMiniMapMod.modSettings.overlayColors.poweredOn;
            else if (battery.StoredEnergy > 1f)
                color = BetterMiniMapMod.modSettings.overlayColors.poweredByBatteries;

            this.DrawThing(battery.parent, color);
        }

        private void DrawThing(Thing thing, Color color)
        {
            foreach (IntVec3 current in thing.OccupiedRect().Cells)
                if (current.InBounds(this.map))
                    base.Texture.SetPixel(current.x, current.z, color);
        }

        public override int OverlayPriority => 850;
    }
}

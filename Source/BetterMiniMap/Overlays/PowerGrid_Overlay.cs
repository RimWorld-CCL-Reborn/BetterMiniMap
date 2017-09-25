using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;

namespace BetterMiniMap.Overlays
{
	public class PowerGrid_Overlay : Overlay
	{
		private static readonly Color poweredColor = GenUI.MouseoverColor;
		private static readonly Color poweredByBatteriesColor = Color.green;
		private static readonly Color notPoweredColor = Color.red;
		private static readonly Color offColor = Color.grey;

        public PowerGrid_Overlay(bool visible) : base(visible) { }

		public override int GetUpdateInterval() => BetterMiniMapMod.settings.overlay_PowerGrid;

		public override void Update()
		{
			base.ClearTexture(false);
            foreach (Building poweredBuilding in Find.VisibleMap.listerBuildings.allBuildingsColonist.Where(b => b.PowerComp != null))
				this.DrawConnection(poweredBuilding.PowerComp);
		}

		private void DrawBattery(CompPowerBattery battery)
		{
			Color color = notPoweredColor;
			if (battery.PowerNet?.CurrentEnergyGainRate() > 1f)
				color = poweredColor;
            else if (battery.StoredEnergy > 1f)
                color = poweredByBatteriesColor;

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
			Color color = offColor;
			if (trader.PowerOn)
				color = poweredColor;
			else if (trader.PowerOutput == 0f)
                color = notPoweredColor;

			Utilities.DrawThing(base.Texture, trader.parent, color);
		}

		private void DrawTransmitter(CompPowerTransmitter transmitter)
		{
			Color color = notPoweredColor;
			if (transmitter.transNet != null)
			{
                if (transmitter.transNet.CurrentEnergyGainRate() > 0f)
                    color = poweredColor;
                else if (transmitter.transNet.CurrentStoredEnergy() > 1f)
                    color = poweredByBatteriesColor;
            }
			Utilities.DrawThing(base.Texture, transmitter.parent, color);
		}

	}
}

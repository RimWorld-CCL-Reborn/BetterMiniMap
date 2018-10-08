using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using System.Xml;

namespace BetterMiniMap
{
    // TODO: how to avoid these extra castings from object -> Pawn/Thing
    public abstract class Selector
    {
        public abstract bool IsValid(object o, Map map);
    }

    public class Default : Selector
    {
        public override bool IsValid(object o, Map map) => true;
    }

    public class ClassSelector : Selector
    {
        public Type classType;
        public override bool IsValid(object o, Map map) => classType.IsAssignableFrom(o.GetType());
    }

    public abstract class ThingSelector : Selector
    {
        public abstract bool IsValid(Thing t);
        public override bool IsValid(object o, Map map) => (o is Thing t) ? IsValid(t) : false;
    }

    public class ThingByDefNameSelector : ThingSelector
    {
        public List<string> defNames;
        public override bool IsValid(Thing t) => this.defNames.Any(n => t.def.defName == n);
    }

    public abstract class PawnSelector : Selector
    {
        public abstract bool IsValid(Pawn p);
        public override bool IsValid(object o, Map map) => (o is Pawn p) ? IsValid(p) : false;
    }

    public class PawnByDefNameSelector : PawnSelector
    {
        public List<string> defNames;
        public override bool IsValid(Pawn p) => this.defNames.Any(n => p.def.defName == n);
    }

    public class PlayerFactionSelector : PawnSelector
    {
        public override bool IsValid(Pawn p) => p.Faction == Faction.OfPlayer;
    }

    public class NonPlayerFactionSelector : PawnSelector
    {
        public override bool IsValid(Pawn p) => p.Faction != Faction.OfPlayer;
    }

    public class HostilePawnsSelector : PawnSelector
    {
        public override bool IsValid(Pawn p) => p.HostileTo(Faction.OfPlayer);
    }

    public class AnimalSelector : PawnSelector
    {
        public override bool IsValid(Pawn p) => p.RaceProps.Animal;
    }

    public class NonAnimalSelector : PawnSelector
    {
        public override bool IsValid(Pawn p) => !p.RaceProps.Animal;
    }

    public class TraderSelector : PawnSelector
    {
        public override bool IsValid(Pawn p) => p.trader != null;
    }

    public class PawnDesignatorSelector : PawnSelector
    {
        public DesignationDef designationDef;
        public override bool IsValid(Pawn p) => Find.CurrentMap.designationManager.DesignationOn(p)?.def == this.designationDef;
    }
}

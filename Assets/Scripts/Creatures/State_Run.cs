namespace EvolvingWilds {
    public class State_Run : CreatureState {

        public State_Run(Creature creature, Creature fromCreature) : base(creature) { }
        
        protected override float DoUtilityCalculation() {
            throw new System.NotImplementedException();
        }
    }
}
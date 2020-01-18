namespace EvolvingWilds {
    public class State_Attack : CreatureState {

        public State_Attack(Creature creature, Creature target) : base(creature) { }
        
        protected override float DoUtilityCalculation() {
            throw new System.NotImplementedException();
        }
    }
}
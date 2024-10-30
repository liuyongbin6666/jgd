namespace Components
{
    public class PortionSpawner : RandomObjectSpawner<PortionComponent>
    {
        protected override void Get(PortionComponent obj) {}
        protected override void Recycle(PortionComponent obj) { }
    }
}
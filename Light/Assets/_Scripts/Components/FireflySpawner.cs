namespace Components
{
    /// <summary>
    /// 萤火虫生成器
    /// </summary>
    public class FireflySpawner : RandomObjectSpawner<FireflyComponent>
    {
        protected override void Get(FireflyComponent obj) => obj.RandomSet();
        protected override void Recycle(FireflyComponent obj) { }
    }
}
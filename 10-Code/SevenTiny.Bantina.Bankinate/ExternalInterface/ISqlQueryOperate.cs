namespace SevenTiny.Bantina.Bankinate
{
    /// <summary>
    /// Sql引擎查询api
    /// </summary>
    public interface ISqlQueryOperate : IQueryOperate
    {
        SqlQueryable<TEntity> Queryable<TEntity>() where TEntity : class;
    }
}

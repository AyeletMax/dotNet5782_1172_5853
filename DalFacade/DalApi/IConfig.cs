namespace DalApi;

public interface IConfig
{
    public TimeSpan RiskRange { get; set; }
    DateTime Clock { get; set; }
    void Reset();
}

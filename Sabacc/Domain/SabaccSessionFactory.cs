namespace Sabacc.Domain;

public class SabaccSessionFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SabaccSessionFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISabaccSession Create(CreateSessionForm sessionForm)
    {
        var session = (ISabaccSession) (sessionForm.SabaccVariant switch
        {
            SabaccVariantType.ClassicSabaccCloudCityRules => _serviceProvider.GetRequiredService<ClassicSabaccCloudCityRules>(),
            SabaccVariantType.CorellianSpikeBlackSpireOutpostRules => _serviceProvider.GetRequiredService<CorellianSpikeBlackSpireOutpostRules>(),
            _ => throw new NotSupportedException()
        });

        session.SetSlots(sessionForm.Slots);

        return session;
    }
}
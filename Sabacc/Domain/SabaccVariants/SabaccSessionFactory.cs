using Sabacc.Domain.SabaccVariants;

namespace Sabacc.Domain;

public class SabaccSessionFactory(IServiceProvider serviceProvider)
{
    public ISabaccSession Create(CreateSessionForm sessionForm)
    {
        var session = (ISabaccSession) (sessionForm.SabaccVariant switch
        {
            SabaccVariantType.ClassicSabaccCloudCityRules => serviceProvider.GetRequiredService<ClassicSabaccCloudCityRules>(),
            SabaccVariantType.CorellianSpikeBlackSpireOutpostRules => serviceProvider.GetRequiredService<CorellianSpikeBlackSpireOutpostRules>(),
            _ => throw new NotSupportedException()
        });

        session.SetSlots(sessionForm.Slots);

        return session;
    }
}
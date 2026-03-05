using Apps.Plunet.Models.Enums;
using Apps.Plunet.Models.FFPicker;

namespace Apps.Plunet.Strategies;

public class StrategyProvider(FfClientProvider clientProvider, PickerMode mode)
{
    private readonly List<IPlunetStrategy> _strategies =
    [
        new RootStrategy(),
        new VirtualEntityStrategy(clientProvider, mode),
        new CustomerStrategy(clientProvider, mode),
        new ResourceStrategy(clientProvider, mode),
        new RequestStrategy(clientProvider, mode),
        new InvoiceStrategy(clientProvider, mode),
        new ReceivableInvoiceStrategy(clientProvider, mode),
        new PayableInvoiceStrategy(clientProvider, mode),
        new OrderStrategy(clientProvider, mode),
        new QuoteStrategy(clientProvider, mode)
    ];

    public IPlunetStrategy? GetStrategy(FfPath path)
    {
        return _strategies.FirstOrDefault(s => s.CanHandle(path));
    }
}
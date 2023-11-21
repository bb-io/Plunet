namespace Apps.Plunet.Models.Customer;

public record ListCustomersResponse(IEnumerable<GetCustomerResponse> Customers);
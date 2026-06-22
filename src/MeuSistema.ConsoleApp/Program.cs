using AulasSOLIDpatterns.Aula02_OpenClosed;

var apple = new Product("Apple", Color.Green, Size.Small);
var spec = new ColorSpecification(Color.Green);

Console.WriteLine(spec.IsSatisfied(apple));
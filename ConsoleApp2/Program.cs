using System.Text;
class Program
{
static void Main(string[] args)
{
try
{

var liquidContainer = new LiquidContainer(200, 100, 200, 1000, true);
var gasContainer = new GasContainer(200, 80, 200, 800, 1.5);
var refrigeratedContainer = new RefrigeratedContainer(300, 110, 300, 900, "Bananas", -5);

var ship = new ContainerShip(20, 5, 5000);

ship.LoadContainer(liquidContainer);
ship.LoadContainer(gasContainer);
ship.LoadContainer(refrigeratedContainer);

Console.WriteLine(ship.ToString());

try
{
liquidContainer.LoadCargo(600);
}
catch (OverfillException oe)
{
Console.WriteLine(oe.Message);
}
}
catch (Exception ex)
{
Console.WriteLine($"An unexpected error occurred: {ex.Message}");
}
}
}
public abstract class Container
{
public double CargoMass { get; protected set; }
public double Height { get; private set; }
public double TareWeight { get; private set; }
public double Depth { get; private set; }
public string SerialNumber { get; protected set; }
public double MaxPayload { get; private set; }
protected Container(double height, double tareWeight, double depth, double maxPayload)
{
Height = height;
TareWeight = tareWeight;
Depth = depth;
MaxPayload = maxPayload;
SerialNumber = SerialNumberGenerator.GenerateSerialNumber(this.GetType().Name.Substring(0, 1));
}
public abstract void LoadCargo(double mass);
public abstract void EmptyCargo();
}
public class LiquidContainer : Container, IHazardNotifier
{
public bool IsHazardous { get; private set; }
public LiquidContainer(double height, double tareWeight, double depth, double maxPayload, bool isHazardous)
: base(height, tareWeight, depth, maxPayload)
{
IsHazardous = isHazardous;
}
public override void LoadCargo(double mass)
{
double allowedCapacity = IsHazardous ? MaxPayload * 0.5 : MaxPayload * 0.9;
if (mass > allowedCapacity)
{
NotifyHazard($"Attempting to overload: {SerialNumber}");
throw new OverfillException($"Attempted to overload container {SerialNumber}.");
}
CargoMass = mass;
}
public override void EmptyCargo()
{
CargoMass = 0;
}
public void NotifyHazard(string message)
{
Console.WriteLine($"Hazard Notification for {SerialNumber}: {message}");
}
}
public class GasContainer : Container, IHazardNotifier
{
public double Pressure { get; private set; }
public GasContainer(double height, double tareWeight, double depth, double maxPayload, double pressure)
: base(height, tareWeight, depth, maxPayload)
{
Pressure = pressure;
}
public override void LoadCargo(double mass)
{
if (mass > MaxPayload)
{
NotifyHazard($"Attempting to overload: {SerialNumber}");
throw new OverfillException($"Attempted to overload container {SerialNumber}.");
}
CargoMass = mass;
}
public override void EmptyCargo()
{
CargoMass = CargoMass * 0.05; 
}
public void NotifyHazard(string message)
{
Console.WriteLine($"Hazard Notification for {SerialNumber}: {message}");
}
}
public class RefrigeratedContainer : Container
{
public string StoredProductType { get; private set; }
public double Temperature { get; private set; }
public RefrigeratedContainer(double height, double tareWeight, double depth, double maxPayload, string storedProductType, double temperature)
: base(height, tareWeight, depth, maxPayload)
{
StoredProductType = storedProductType;
Temperature = temperature;
}
public override void LoadCargo(double mass)
{
if (mass > MaxPayload)
{
throw new OverfillException($"Attempted to overload container {SerialNumber}.");
}
CargoMass = mass;
}
public override void EmptyCargo()
{
CargoMass = 0;
}
}
public interface IHazardNotifier
{
void NotifyHazard(string message);
}
public class ContainerShip
{
private List<Container> loadedContainers = new List<Container>();
public double MaxSpeed { get; private set; }
public int MaxContainerCapacity { get; private set; }
public double MaxWeightCapacity { get; private set; }
public ContainerShip(double maxSpeed, int maxContainerCapacity, double maxWeightCapacity)
{
MaxSpeed = maxSpeed;
MaxContainerCapacity = maxContainerCapacity;
MaxWeightCapacity = maxWeightCapacity;
}
public void LoadContainer(Container container)
{
if (loadedContainers.Count >= MaxContainerCapacity || loadedContainers.Sum(c => c.CargoMass) + container.CargoMass > MaxWeightCapacity * 1000)
{
throw new InvalidOperationException("Cannot add more containers: capacity or weight limit reached.");
}
loadedContainers.Add(container);
}
public override string ToString()
{
StringBuilder sb = new StringBuilder();
sb.AppendLine($"Container Ship - Max Speed: {MaxSpeed} knots, Max Container Capacity: {MaxContainerCapacity}, Max Weight Capacity: {MaxWeightCapacity} tons");
foreach (var container in loadedContainers)
{
sb.AppendLine($" - {container.GetType().Name} {container.SerialNumber}: Cargo Mass = {container.CargoMass} kg");
}
return sb.ToString();
}
}
public static class SerialNumberGenerator
{
private static Dictionary<string, int> NextNumbers = new Dictionary<string, int>();
public static string GenerateSerialNumber(string prefix)
{
if (!NextNumbers.ContainsKey(prefix))
{
NextNumbers[prefix] = 1;
}
var serialNumber = $"KON-{prefix}-{NextNumbers[prefix]}";
NextNumbers[prefix]++;
return serialNumber;
}
}
public class OverfillException : Exception
{
public OverfillException(string message) : base(message) { }
}
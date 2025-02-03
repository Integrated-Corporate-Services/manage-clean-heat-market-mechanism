using Desnz.Chmm.McsSynchronisation.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.McsSynchronisation.Api.Infrastructure;

public class McsSynchronisationContextSeed
{
    public async Task SeedAsync(McsSynchronisationContext context)
    {
        using (context)
        {
            //await context.Database.EnsureCreatedAsync();
            try
            {
                await context.Database.MigrateAsync();
                await SeedData(context);
            }
            catch { }
        }
    }

    private async Task SeedData(McsSynchronisationContext context)
    {
        context.Add(new TechnologyType() { Id = 1, Description = "Air Source Heat Pump" });
        context.Add(new TechnologyType() { Id = 2, Description = "Biomass" });
        context.Add(new TechnologyType() { Id = 3, Description = "Ground Source Heat Pump" });
        context.Add(new TechnologyType() { Id = 4, Description = "Ground/Water source Heat Pump" });
        context.Add(new TechnologyType() { Id = 6, Description = "Hot Water Heat Pump" });
        context.Add(new TechnologyType() { Id = 7, Description = "Hydro" });
        context.Add(new TechnologyType() { Id = 8, Description = "Micro CHP" });
        context.Add(new TechnologyType() { Id = 9, Description = "Solar Keymark" });
        context.Add(new TechnologyType() { Id = 10, Description = "Solar Photovoltaic" });
        context.Add(new TechnologyType() { Id = 13, Description = "Wind Turbine" });
        context.Add(new TechnologyType() { Id = 14, Description = "Exhaust Air Heat Pump" });
        context.Add(new TechnologyType() { Id = 15, Description = "Water Source Heat Pump" });
        context.Add(new TechnologyType() { Id = 16, Description = "Gas Absorbtion Heat Pump" });
        context.Add(new TechnologyType() { Id = 17, Description = "Solar Assisted Heat Pump" });
        context.Add(new TechnologyType() { Id = 18, Description = "Solar Thermal" });
        context.Add(new TechnologyType() { Id = 19, Description = "Battery Storage" });

        context.Add(new AirTypeTechnology() { Id = 1, Description = "Air to Air Source Heat Pump" });
        context.Add(new AirTypeTechnology() { Id = 2, Description = "Air to Water Source Heat Pump" });

        context.Add(new InstallationAge() { Id = 1, Description = "Installed at the same time" });
        context.Add(new InstallationAge() { Id = 2, Description = "Up to 3 months" });

        context.Add(new NewBuildOption() { Id = 1, Description = "Yes" });
        context.Add(new NewBuildOption() { Id = 2, Description = "No" });

        context.Add(new RenewableSystemDesign() { Id = 1, Description = "Space Heat, DHW and another purpose" });
        context.Add(new RenewableSystemDesign() { Id = 2, Description = "Space Heat and another purpose" });
        context.Add(new RenewableSystemDesign() { Id = 3, Description = "DHW and another purpose" });
        context.Add(new RenewableSystemDesign() { Id = 4, Description = "Space heat and DHW" });
        context.Add(new RenewableSystemDesign() { Id = 5, Description = "Space heat only" });
        context.Add(new RenewableSystemDesign() { Id = 6, Description = "DHW only" });
        context.Add(new RenewableSystemDesign() { Id = 7, Description = "Another purpose only" });

        context.Add(new AlternativeSystemFuelType() { Id = 1, Description = "Anthracite" });
        context.Add(new AlternativeSystemFuelType() { Id = 2, Description = "Biogas (Landfill) Community Heating Only" });
        context.Add(new AlternativeSystemFuelType() { Id = 3, Description = "Bulkwood Pellets" });
        context.Add(new AlternativeSystemFuelType() { Id = 4, Description = "Coal" });
        context.Add(new AlternativeSystemFuelType() { Id = 5, Description = "Duel Fuel Mineral and Wood" });
        context.Add(new AlternativeSystemFuelType() { Id = 6, Description = "Electricity" });
        context.Add(new AlternativeSystemFuelType() { Id = 7, Description = "LPG" });
        context.Add(new AlternativeSystemFuelType() { Id = 8, Description = "Mains Gas" });
        context.Add(new AlternativeSystemFuelType() { Id = 9, Description = "Oil" });
        context.Add(new AlternativeSystemFuelType() { Id = 10, Description = "Waste Combustion (Community Heating Only)" });
        context.Add(new AlternativeSystemFuelType() { Id = 11, Description = "Wood Chips" });
        context.Add(new AlternativeSystemFuelType() { Id = 12, Description = "Wood Logs" });
        context.Add(new AlternativeSystemFuelType() { Id = 13, Description = "None" });
        context.Add(new AlternativeSystemFuelType() { Id = 14, Description = "Other" });

        context.Add(new AlternativeSystemType() { Id = 1, Description = "Back Boiler" });
        context.Add(new AlternativeSystemType() { Id = 2, Description = "Combi Boiler" });
        context.Add(new AlternativeSystemType() { Id = 3, Description = "Community Heating" });
        context.Add(new AlternativeSystemType() { Id = 4, Description = "Condensing Boiler" });
        context.Add(new AlternativeSystemType() { Id = 5, Description = "Condensing Combi Boiler" });
        context.Add(new AlternativeSystemType() { Id = 6, Description = "Combined Primary Storage Units (CPSU) " });
        context.Add(new AlternativeSystemType() { Id = 7, Description = "Electric Panel, Convector or Radiant Heaters" });
        context.Add(new AlternativeSystemType() { Id = 8, Description = "Electric Underfloor / Ceiling Heating" });
        context.Add(new AlternativeSystemType() { Id = 9, Description = "Fire or Wall Heater" });
        context.Add(new AlternativeSystemType() { Id = 10, Description = "Heat Pump" });
        context.Add(new AlternativeSystemType() { Id = 11, Description = "High or Unknown Thermal Capacity Boiler" });
        context.Add(new AlternativeSystemType() { Id = 12, Description = "None (or Default - Portable electric heaters )" });
        context.Add(new AlternativeSystemType() { Id = 13, Description = "Open Fire with Back Boiler" });
        context.Add(new AlternativeSystemType() { Id = 14, Description = "Range Cooker Boiler (Integral Oven and Boiler)" });
        context.Add(new AlternativeSystemType() { Id = 15, Description = "Standard Boiler" });
        context.Add(new AlternativeSystemType() { Id = 16, Description = "Storage Heaters" });
        context.Add(new AlternativeSystemType() { Id = 17, Description = "Closed Room Heater (Stove) with Back Boiler" });
        context.Add(new AlternativeSystemType() { Id = 18, Description = "Wall or Floor Mounted Boiler (Balanced or Open Flue) - Pre 1998" });
        context.Add(new AlternativeSystemType() { Id = 19, Description = "Warm Air Systems" });
        context.Add(new AlternativeSystemType() { Id = 20, Description = "Other" });

        context.Add(new Manufacturer() { Id = 1, Description = "1st Sunflower Renewable Energy" });
        context.Add(new Manufacturer() { Id = 2, Description = "24 Sun - Solar Systems, Ltd." });
        context.Add(new Manufacturer() { Id = 3, Description = "3S Swiss Solar Systems AG" });
    }
}

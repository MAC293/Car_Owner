using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RESTfulAPI.Models;
using RESTfulAPI.Services;
using System.Security.Claims;

namespace RESTfulAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private CacheService _CacheService;
        private String _ClaimID;

        public CarController(CacheService cacheService)
        {
            CacheService = cacheService;
        }

        public String ClaimID
        {
            get { return _ClaimID; }
            set { _ClaimID = value; }
        }
        public CacheService CacheService
        {
            get { return _CacheService; }
            set { _CacheService = value; }
        }

        #region POST
        //POST: creates a new car
        [HttpPost]
        [Route("Create")]
        //This ensures that the JWT token is validated before the method is executed.
        [Authorize]
        public async Task<IActionResult> POST(VehicleService newVehicleService)
        {
            using (RestdatabaseContext context = new RestdatabaseContext())
            {
                if (!hasClaim())
                {
                    return Unauthorized();
                }

                var vehicleDAL = await context.Vehicles.FirstOrDefaultAsync(vehicle => vehicle.Patent == newVehicleService.Patent && vehicle.Driver == ClaimID);

                if (vehicleDAL != null)
                {
                    return BadRequest();
                }

                context.Vehicles.Add(MappingPOST(newVehicleService, ClaimID));
                await context.SaveChangesAsync();

                //return CreatedAtAction(nameof(GET), new { patent = newCar.Patent }, newCar);
                return Created();
            }
        }

        private Vehicle MappingPOST(VehicleService vehicleService, String driver)
        {
            var newVehicle = new Vehicle
            {
                Patent = vehicleService.Patent,
                Brand = vehicleService.Brand,
                Model = vehicleService.Model,
                Type = vehicleService.Type,
                Year = vehicleService.Year,
                Driver = driver
            };

            return newVehicle;
        }
        #endregion

        #region GET
        //GET: display an specific vehicle by patent
        [HttpGet("GETVehicle/{patent}")]
        //[Authorize]: This ensures that the JWT token is validated before the method is executed
        [Authorize]
        public async Task<ActionResult<VehicleService>> GET(String patent)
        {
            using (RestdatabaseContext context = new RestdatabaseContext())
            {
                if (!hasClaim())
                {
                    return Unauthorized();
                }

                //var cachedVehicle = CacheService.Get<VehicleService>(patent);
                var cachedVehicle = CacheService.Get<VehicleService>($"user:{ClaimID}:vehicle:{patent}");

                if (cachedVehicle != null)
                {
                    return cachedVehicle;
                }

                var carDAL = await context.Vehicles.FirstOrDefaultAsync(car => car.Patent == patent && car.Driver == ClaimID);

                if (carDAL != null)
                {
                    //var vehicleService = MappingGET(carDAL);
                    var vehicleService = MappingCheckedVehicle(carDAL);

                    //CacheService.Set(patent, vehicleService);
                    CacheService.Set(ClaimID, patent, vehicleService);

                    return vehicleService;
                }

                return NotFound();
            }
        }

        private ActionResult<VehicleService> MappingGET(Vehicle aVehicle)
        {

            var aSVehicle = new VehicleService
            {

                Patent = aVehicle.Patent,
                Brand = aVehicle.Brand,
                Model = aVehicle.Model,
                Type = aVehicle.Type,
                Year = (int)aVehicle.Year
            };

            return aSVehicle;
        }
        #endregion

        #region PUT Car
        //PUT: edits a car
        [HttpPut]
        [Route("Update")]
        [Authorize]
        public async Task<IActionResult> PUT(VehicleService updateCar)
        {
            using (RestdatabaseContext context = new RestdatabaseContext())
            {

                if (!hasClaim())
                {
                    return Unauthorized();
                }

                var carDAL = await context.Vehicles.FirstOrDefaultAsync(car => car.Patent == updateCar.Patent && car.Driver == ClaimID);

                if (carDAL == null)
                {
                    return Conflict();
                }

                MappingPUT(carDAL, updateCar);

                await context.SaveChangesAsync();

                hasVehicle(updateCar);

                return NoContent();
            }
        }

        private void MappingPUT(Vehicle vehicleDAL, VehicleService vehicleService)
        {
            vehicleDAL.Brand = vehicleService.Brand;
            vehicleDAL.Model = vehicleService.Model;
            vehicleDAL.Type = vehicleService.Type;
            vehicleDAL.Year = vehicleService.Year;
        }

        public void hasVehicle(VehicleService updateCar)
        {
            using (RestdatabaseContext context = new RestdatabaseContext())
            {
                //All
                List<VehicleService>? allList = CacheService.GetAlt<List<VehicleService>>($"user:{ClaimID}:vehicle:all");

                if (allList != null)
                {
                    if (allList.Any(car => car.Patent == updateCar.Patent))
                    {
                        ClearCache($"user:{ClaimID}:vehicle:all");

                        var carDAL = context.Vehicles.Where(car => car.Driver == ClaimID).ToList();

                        if (carDAL.Any())
                        {
                            var vehicleCacheList = MappingCheckedList(carDAL);

                            CacheService.Set(ClaimID, "all", vehicleCacheList);
                        }
                    }
                }

                //Type
                List<VehicleService>? typeList = CacheService.GetAlt<List<VehicleService>>($"user:{ClaimID}:vehicle:{updateCar.Type}");

                if (typeList != null)
                {
                    if (typeList.Any(car => car.Patent == updateCar.Patent))
                    {
                        ClearCache($"user:{ClaimID}:vehicle:{updateCar.Type}");

                        var carDAL = context.Vehicles.Where(car => car.Type == updateCar.Type && car.Driver == ClaimID).ToList();

                        if (carDAL.Any())
                        {
                            var vehicleCacheList = MappingCheckedList(carDAL);

                            CacheService.Set(ClaimID, updateCar.Type, vehicleCacheList);
                        }
                    }
                }

                //Patent
                VehicleService? vehicle = CacheService.GetAlt<VehicleService>($"user:{ClaimID}:vehicle:{updateCar.Patent}");

                if (vehicle != null)
                {
                    ClearCache($"user:{ClaimID}:vehicle:{updateCar.Patent}");

                    var vehicleDAL = context.Vehicles.FirstOrDefault(vehicle => vehicle.Patent == updateCar.Patent && vehicle.Driver == ClaimID);

                    if (vehicleDAL != null)
                    {
                        var vehicleCache = MappingCheckedVehicle(vehicleDAL);

                        CacheService.Set(ClaimID, updateCar.Patent, vehicleCache);
                    }

                }
            }
        }

        private VehicleService MappingCheckedVehicle(Vehicle aVehicle)
        {
            var carServiceList = new VehicleService
            {
                Patent = aVehicle.Patent,
                Brand = aVehicle.Brand,
                Model = aVehicle.Model,
                Type = aVehicle.Type,
                Year = (int)aVehicle.Year
            };

            return carServiceList;
        }

        private List<VehicleService> MappingCheckedList(List<Vehicle> aVehicle)
        {
            var carServiceList = aVehicle.Select(car => new VehicleService
            {
                Patent = car.Patent,
                Brand = car.Brand,
                Model = car.Model,
                Type = car.Type,
                Year = (int)car.Year

            }).ToList();

            return carServiceList;
        }

        #endregion

        private ActionResult<List<VehicleService>> MappingGETType(List<Vehicle> aVehicle)
        {
            var carServiceList = aVehicle.Select(car => new VehicleService
            {
                Patent = car.Patent,
                Brand = car.Brand,
                Model = car.Model,
                Type = car.Type,
                Year = (int)car.Year

            }).ToList();

            return carServiceList;
        }

        private List<VehicleService> MappingGETUpdate(List<Vehicle> aVehicle)
        {
            var carServiceList = aVehicle.Select(car => new VehicleService
            {
                Patent = car.Patent,
                Brand = car.Brand,
                Model = car.Model,
                Type = car.Type,
                Year = (int)car.Year

            }).ToList();

            return carServiceList;
        }

        #region GET All
        //GET: displays all
        [HttpGet]
        [Route("GETAll")]
        [Authorize]
        public async Task<ActionResult<List<VehicleService>>> GETAllCars()
        {
            using (RestdatabaseContext context = new RestdatabaseContext())
            {
                if (!hasClaim())
                {
                    return Unauthorized();
                }

                var cachedVehicles = CacheService.GetAlt<List<VehicleService>>($"user:{ClaimID}:vehicle:all");

                if (cachedVehicles != null)
                {
                    return cachedVehicles;
                }

                var carDAL = await context.Vehicles.Where(car => car.Driver == ClaimID).ToListAsync();

                if (carDAL.Any())
                {
                    var vehicleServiceList = MappingGETAllAlt(carDAL);

                    CacheService.Set(ClaimID, "all", vehicleServiceList);

                    return vehicleServiceList;
                }

                return NotFound();
            }
        }

        private List<VehicleService> MappingGETAllAlt(List<Vehicle> aVehicle)
        {
            var carServiceList = aVehicle.Select(car => new VehicleService
            {
                Patent = car.Patent,
                Brand = car.Brand,
                Model = car.Model,
                Type = car.Type,
                Year = (int)car.Year

            }).ToList();

            return carServiceList;
        }

        private ActionResult<List<VehicleService>> MappingGETAll(List<Vehicle> aVehicle)
        {
            var carServiceList = aVehicle.Select(car => new VehicleService
            {
                Patent = car.Patent,
                Brand = car.Brand,
                Model = car.Model,
                Type = car.Type,
                Year = (int)car.Year

            }).ToList();

            return carServiceList;
        }
        #endregion

        #region DELETE
        //DELETE: deletes a car by patent
        [HttpDelete("Delete/{patent}")]
        [Authorize]
        public async Task<IActionResult> DELETE(String patent)
        {
            using (RestdatabaseContext context = new RestdatabaseContext())
            {
                if (!hasClaim())
                {
                    return Unauthorized();
                }

                var carDAL = await context.Vehicles.FirstOrDefaultAsync(car => car.Patent == patent && car.Driver == ClaimID);

                if (carDAL != null)
                {
                    CheckDelete(carDAL);

                    context.Vehicles.Remove(carDAL);
                    await context.SaveChangesAsync();

                    return NoContent();
                }

                return NotFound();
            }
        }

        private void CheckDelete(Vehicle vehicleRemove)
        {
            using (RestdatabaseContext context = new RestdatabaseContext())
            {
                //All
                List<VehicleService>? allList = CacheService.GetAlt<List<VehicleService>>($"user:{ClaimID}:vehicle:all");

                if (allList != null)
                {
                    if (allList.Any(car => car.Patent == vehicleRemove.Patent))
                    {
                        allList.RemoveAll(car => car.Patent == vehicleRemove.Patent);

                        CacheService.Set(ClaimID, "all", allList);

                    }
                }

                //Type
                List<VehicleService>? typeList = CacheService.GetAlt<List<VehicleService>>($"user:{ClaimID}:vehicle:{vehicleRemove.Type?.Trim()}");

                if (typeList != null)
                {
                    if (typeList.Any(car => car.Patent == vehicleRemove.Patent))
                    {
                        typeList.RemoveAll(car => car.Patent == vehicleRemove.Patent);

                        CacheService.Set(ClaimID, vehicleRemove.Type?.Trim(), typeList);
                    }
                }

                //Patent
                var vehicle = CacheService.GetAlt<VehicleService>($"user:{ClaimID}:vehicle:{vehicleRemove.Patent?.Trim()}");

                String key = $"user:{ClaimID}:vehicle:{vehicleRemove.Patent?.Trim()}";

                if (vehicle != null)
                {
                    ClearCache(key);
                }
            }
        }
        #endregion

        private void ClearCache(String key)
        {
            CacheService.Remove(key);
        }

        #region Claim
        private Boolean hasClaim()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return false;
            }

            ClaimID = userIdClaim.Value;

            return true;
        }
        #endregion

        #region GET Cars Type
        [HttpGet("GETVehicles/{type}")]
        [Authorize]
        public async Task<ActionResult<List<VehicleService>>> GETCars(String type)
        {
            using (RestdatabaseContext context = new RestdatabaseContext())
            {
                if (!hasClaim())
                {
                    return Unauthorized();
                }

                var cachedVehicles = CacheService.GetAlt<List<VehicleService>>($"user:{ClaimID}:vehicle:{type}");

                if (cachedVehicles != null)
                {
                    return cachedVehicles;
                }

                var carDAL = await context.Vehicles.Where(car => car.Type == type && car.Driver == ClaimID).ToListAsync();

                if (carDAL.Any())
                {
                    var vehicleServiceList = MappingGETAllAlt(carDAL);

                    CacheService.Set(ClaimID, type, vehicleServiceList);

                    return vehicleServiceList;
                }

                return NotFound();
            }
        }
        #endregion
    }
}

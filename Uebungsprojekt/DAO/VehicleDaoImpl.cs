﻿using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uebungsprojekt.Models;

namespace Uebungsprojekt.DAO
{
    public class VehicleDaoImpl : VehicleDao
    {
        private IMemoryCache _cache;

        public VehicleDaoImpl(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// Adds a vehicle to the Vehiclelist if there is one, else it creates a new List and adds the vehicle
        /// </summary>
        /// <param name="vehicle">Vehicle that is to be added</param>
        /// <returns>the added Vehicle</returns>
        public Vehicle Create(Vehicle vehicle)
        {
            if (_cache.TryGetValue("CreateVehicle", out List<Vehicle> createdVehicles))
            {
                createdVehicles.Add(vehicle);
                return vehicle;
            }
            else
            {
                createdVehicles = new List<Vehicle> { vehicle };
                _cache.Set("CreateVehicle", createdVehicles);
                return vehicle;
            }
        }

        /// <summary>
        /// Delets the Vehicle with specified Id
        /// </summary>
        /// <param name="_Id">Vehicle Id</param>
        /// <returns>true if found and deleted, false else</returns>
        public bool Delete(int _Id)
        {
            if (_cache.TryGetValue("CreateVehicle", out List<Vehicle> createdVehicles))
            {
                if (GetById(_Id) == null)
                {
                    return false;
                }
                createdVehicles.Remove(GetById(_Id));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the List of Vehicles in Cache if there is one, else it creates a new List and returns it
        /// </summary>
        /// <returns>List of Vehicles</returns>
        public List<Vehicle> GetAll()
        {
            if (_cache.TryGetValue("CreateVehicle", out List<Vehicle> createdVehicles))
            {
                return createdVehicles;
            }
            else
            {
                createdVehicles = new List<Vehicle>();
                _cache.Set("CreateVehicle", createdVehicles);
                return createdVehicles;
            }
        }

        /// <summary>
        /// Finds a Vehicle with specified ID and returns it
        /// </summary>
        /// <param name="_Id">Vehicle Id</param>
        /// <returns>Vehicle with specified Id on success and null on failure</returns>
        public Vehicle GetById(int _Id)
        {
            if (_cache.TryGetValue("CreateVehicle", out List<Vehicle> createdVehicles))
            {

                return createdVehicles.Find(x => x.id == _Id);
            }
            else
            {
                return null;
            }
        }
    }
}

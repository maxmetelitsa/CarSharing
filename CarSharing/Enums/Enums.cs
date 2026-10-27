using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarSharing.Enums
{
    public enum Fuel
    {
        Petrol,
        Diesel,
        Gas,
        Electricity,
        Hybrid
    };
    public enum Transmission
    {
        Automatic,
        Manual,
    };
    public enum CarType
    {
        None,
        Economy,
        Business,
        Coupe,
        Limousine
    }
    public enum Brand
    {
        None, Audi, Bentley, BMW, Chevrolet, Ford, Honda, Hyundai, Jaguar, Kia, Lamborghini, Lexus, Mazda, MercedesBenz, Nissan, Porsche, Subaru, Tesla, Toyota, Volkswagen
    }

    /// <summary>Result of a sign-in attempt.</summary>
    public enum LoginResult
    {
        Failed = 0,
        Customer = 1,
        Administrator = 2
    }
}

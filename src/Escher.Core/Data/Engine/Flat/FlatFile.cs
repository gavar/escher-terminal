using System;
using System.IO;
using FlatFile.Delimited;
using FlatFile.Delimited.Attributes.Infrastructure;
using FlatFile.Delimited.Implementation;

namespace Escher.Data.Engine.Flat
{
    public static class FlatFile
    {
        public static readonly DelimitedLayoutDescriptorProvider Provider = new DelimitedLayoutDescriptorProvider();

        public static IDelimitedLayoutDescriptor GetDescriptor(Type type)
        {
            return Provider.GetDescriptor(type);
        }

        public static IDelimitedLineBuilder GetBuilder(IDelimitedLayoutDescriptor descriptor)
        {
            return new FlatFileRepositoryLineBuilder(descriptor);
        }

        public static string GetStorePath(IDelimitedLayoutDescriptor descriptor)
        {
            return Path.Join("store", descriptor.TargetType.Name + ".csv");
        }
    }
}
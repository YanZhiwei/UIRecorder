using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using UIRecorder.Models;

namespace UIRecorder;

internal class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        //CreateMap<AutomationElement, UiAccessibilityElement>()
        //    .ForMember(destination => destination.BoundingRectangle,
        //        opts => opts.MapFrom(source => source.Properties.BoundingRectangle.ValueOrDefault))
        //    .ForMember(destination => destination.IsOffscreen,
        //        opts => opts.MapFrom(source => source.Properties.IsOffscreen.ValueOrDefault))
        //    .ForMember(destination => destination.IsEnabled,
        //        opts => opts.MapFrom(source => source.Properties.IsEnabled.ValueOrDefault))
        //    .ForMember(destination => destination.ActualHeight,
        //        opts => opts.MapFrom(source => source.ActualHeight))
        //    .ForMember(destination => destination.ActualWidth,
        //        opts => opts.MapFrom(source => source.ActualWidth))
        //    .ForMember(destination => destination.IsDialog,
        //        opts => opts.MapFrom(source => source.Properties.IsDialog.ValueOrDefault));

        CreateMap<ControlType, UiAccessibilityControlType>()
            .ConvertUsingEnumMapping(opt => opt
                .MapValue(ControlType.Pane, UiAccessibilityControlType.Pane))
            .ReverseMap(); // to support Destination to Source mapping, including custom mappings of ConvertUsingEnumMapping
    }
}

using System;
using NetTopologySuite.Geometries;

namespace TRAISI.SDK.Library.ResponseTypes {
	public interface ITimelineResponse : IResponseType {
		string Purpose { get; set; }

		string Name { get; set; }

		Point Location {get;set;}

		DateTimeOffset TimeA { get; set; }

		DateTimeOffset TimeB { get; set; }

		int? Order { get; set; }
	}
}
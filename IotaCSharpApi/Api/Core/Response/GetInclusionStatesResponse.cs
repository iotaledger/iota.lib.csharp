﻿using System.Collections.Generic;

namespace Iota.Lib.CSharp.Api.Core
{
    /// <summary>
    /// This class represents the response of <see cref="GetInclusionStatesRequest"/>
    /// </summary>
    /// <seealso cref="Iota.Lib.CSharp.Api.Core.IotaResponse" />
    public class GetInclusionStatesResponse : IotaResponse
    {
        /// <summary>
        /// Gets or sets the states.
        /// </summary>
        /// <value>
        /// The states.
        /// </value>
        public List<bool> States { get; set; }
    }
}
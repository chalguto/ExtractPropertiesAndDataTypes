  /// <summary>
  /// Extracts property information (name and data type) from the paged query results.
  /// This method analyzes each IQueryResult item to provide detailed information about
  /// the properties and their corresponding data types returned.
  /// </summary>
  /// <param name="results">The list of IQueryResult items to analyze.</param>
  /// <returns>A dictionary containing property names as keys and their data types as values.</returns>
  private Dictionary<string, string> ExtractPropertiesAndDataTypes(List<IQueryResult> results)
  {
      var propertyInfo = new Dictionary<string, string>();

      if (results == null || !results.Any())
      {
          return propertyInfo;
      }

      // Analyze the first result to get all available properties
      var firstResult = results.First();

      foreach (var property in firstResult.Properties)
      {
          string propertyName = property.LocalName ?? property.QueryName ?? property.DisplayName ?? "Unknown";
          string dataType = "Unknown";

          try
          {
              // Try to infer data type from the actual value
              if (property.FirstValue != null)
              {
                  dataType = property.FirstValue.GetType().Name;
              }
              else if (property.Values != null && property.Values.Count > 0)
              {
                  // Check if it's a multi-valued property
                  var firstNonNullValue = property.Values.FirstOrDefault(v => v != null);
                  if (firstNonNullValue != null)
                  {
                      dataType = firstNonNullValue.GetType().Name;
                      if (property.Values.Count > 1)
                      {
                          dataType += "[]"; // Indicate it's an array/collection
                      }
                  }
              }
          }
          catch (Exception)
          {
              // If we can't determine the type, keep "Unknown"
              dataType = "Unknown";
          }

          // Store the property information
          if (!propertyInfo.ContainsKey(propertyName))
          {
              propertyInfo.Add(propertyName, dataType);
          }
      }

      return propertyInfo;
  }
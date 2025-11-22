# Client Avatar - ID Document Integration Notes

## Current Implementation
The client avatar now uses the standard `GlowAvatar` control displaying initials.

## Future ID Document Integration

When implementing ID document upload functionality, follow these steps to extract and display the ID photo:

### 1. Add Document Type to Domain Model
Add a `DocumentType` property to `Document.cs`:
```csharp
public string DocumentType { get; set; } = ""; // e.g., "ID", "Contract", "Waiver", etc.
```

### 2. Update Database Schema
Add a `documentType` column to the documents table in `DatabaseManager.cs`:
```csharp
EnsureColumnExists(connection, "documents", "documentType", "TEXT DEFAULT ''");
```

### 3. ID Photo Extraction
When an ID document is uploaded:
- Use an OCR/image processing library to detect and extract the face photo from the ID
- Save the extracted photo to the `ClientPhotos` directory
- Update the client's `PhotoPath` with the extracted photo path
- Store the full ID scan in the documents table

### 4. Enhance GlowAvatar Control
Modify `GlowAvatar.cs` to support optional photo display:
```csharp
public static readonly DependencyProperty PhotoPathProperty =
    DependencyProperty.Register("PhotoPath", typeof(string), typeof(GlowAvatar), 
        new PropertyMetadata(null, OnPhotoPathChanged));

private static void OnPhotoPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
{
    var control = (GlowAvatar)d;
    var photoPath = e.NewValue as string;
    
    if (!string.IsNullOrWhiteSpace(photoPath) && File.Exists(photoPath))
    {
        // Show photo instead of initials
        var imageBrush = new ImageBrush(new BitmapImage(new Uri(photoPath)));
        control._ellipse.Fill = imageBrush;
    }
    else
    {
        // Show initials with background color
        control._ellipse.Fill = control.BackgroundColor;
    }
}
```

### 5. Update AddEditClientView.xaml
Bind the PhotoPath to the GlowAvatar:
```xml
<controls:GlowAvatar Grid.Column="1" 
                    Size="120" 
                    Initials="{Binding AvatarInitials}"
                    PhotoPath="{Binding Client.PhotoPath}"
                    Margin="20,0,0,0" 
                    VerticalAlignment="Center"/>
```

### 6. Document Upload Handler
In the document upload logic:
```csharp
private void OnIdDocumentUploaded(int clientId, string idScanPath)
{
    // Extract photo from ID using image processing
    var extractedPhotoPath = ExtractIdPhoto(idScanPath);
    
    // Update client's photo path
    var client = _clientRepository.Get(clientId);
    if (client != null)
    {
        client.PhotoPath = extractedPhotoPath;
        _clientRepository.Update(client);
    }
}
```

## Benefits of This Approach
- ✅ Avatars show initials by default (current state)
- ✅ Seamless transition to ID photo when document is uploaded
- ✅ Database already configured with PhotoPath field
- ✅ No UI changes needed - photo displays automatically when available
- ✅ Maintains consistency across the application

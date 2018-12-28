// Run:
// * With Photoshop open, select File > Scripts > ExportAllImages

// What are the sizes used for
// http://www.creativefreedom.co.uk/icon-designers-blog/windows-phone-8-1-app-icon-size-guide/
 
// Turn debugger on. 0 is off.
$.level = 1;
 
 // Function to launch the "Layer > Rasterize > with style"
// Produced with the JavaScript listener
function raterizeLayerStyle()
{
    var idrasterizeLayer = stringIDToTypeID( "rasterizeLayer" );
    var desc5 = new ActionDescriptor();
    var idnull = charIDToTypeID( "null" );
        var ref4 = new ActionReference();
        var idLyr = charIDToTypeID( "Lyr " );
        var idOrdn = charIDToTypeID( "Ordn" );
        var idTrgt = charIDToTypeID( "Trgt" );
        ref4.putEnumerated( idLyr, idOrdn, idTrgt );
    desc5.putReference( idnull, ref4 );
    var idWhat = charIDToTypeID( "What" );
    var idrasterizeItem = stringIDToTypeID( "rasterizeItem" );
    var idlayerStyle = stringIDToTypeID( "layerStyle" );
    desc5.putEnumerated( idWhat, idrasterizeItem, idlayerStyle );
    executeAction( idrasterizeLayer, desc5, DialogModes.NO );
}


try
{
    if ( app.documents.length <= 0 )    
        throw "No active documents";
    
    var destFolder = Folder.selectDialog( "Choose an output folder");
    if (destFolder == null)
    {
      // User canceled, just exit
      throw "";
    }


    var icons = [
      {"name": "Logo070.scale-80", "size":56},
      {"name": "Logo070.scale-100", "size":70},
      {"name": "Logo070.scale-140", "size":98},
      {"name": "Logo070.scale-180", "size":126},
      
      {"name": "Logo150.scale-80", "size":120},
      {"name": "Logo150.scale-100", "size":150},
      {"name": "Logo150.scale-140", "size":210},
      {"name": "Logo150.scale-180", "size":270},
      
      {"name": "Logo310.scale-80", "size":248},
      {"name": "Logo310.scale-100", "size":310},
      {"name": "Logo310.scale-140", "size":434},
      {"name": "Logo310.scale-180", "size":558},

      {"name": "Logo030.scale-80", "size":24},
      {"name": "Logo030.scale-100", "size":30},
      {"name": "Logo030.scale-140", "size":42},
      {"name": "Logo030.scale-180", "size":54},

      {"name": "Logo.16", "size":16},
      {"name": "Logo.32", "size":32},
      {"name": "Logo.48", "size":48},
      {"name": "Logo.256", "size":256},

      {"name": "LogoStore.scale-100", "size":50},
      {"name": "LogoStore.scale-140", "size":70},
      {"name": "LogoStore.scale-180", "size":90},

      {"name": "LogoBadge.scale-100", "size":24},
      {"name": "LogoBadge.scale-140", "size":33},
      {"name": "LogoBadge.scale-180", "size":43},


    ];
 
    var icon;
    var startState;
    for (i = 0; i < icons.length; i++) 
    {
        icon = icons[i];
        
        // Get current document and current layer
        var docRef = app.activeDocument;
        var activeLay = docRef.activeLayer;
 
        startState = docRef.activeHistoryState;
 
        //activeLay.rasterize(RasterizeType.ENTIRELAYER) ;
        raterizeLayerStyle();
         
        // resize image
        docRef.resizeImage(
            icon.size, icon.size, // width, height
            null, ResampleMethod.BICUBICSHARPER);

        //Options to export to PNG files
        var options = new ExportOptionsSaveForWeb();
  
        options.format = SaveDocumentType.PNG;
        options.PNG8 = false;
        options.transparency = true;
        options.optimized = true;
    
        //Export Save for Web in the current folder
        docRef.exportDocument(new File(destFolder + "/" + icon.name + ".png"),ExportType.SAVEFORWEB, options);

        docRef.activeHistoryState = startState; // undo resize
    }
 
    alert("Images created!");
}
catch (exception)
{
    // Show debug message, then quit
    if ((exception != null) && (exception != ""))
        alert(exception);
}
finally
{
}

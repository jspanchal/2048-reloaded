// Install - ExportAllImages.jsx to:
//   Win: C:\Program Files\Adobe\Adobe Utilities\ExtendScript Toolkit CS5\SDK
// * Restart Photoshop
// * Just modify & save, no need to resart Photoshop once it's installed.
//
// Run:
// * With Photoshop open, select File > Scripts > ExportAllImages
 
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
      {"name": "Logo044.scale-100", "size":44},
      {"name": "Logo044.scale-140", "size":62},
      {"name": "Logo044.scale-240", "size":106},
	  
      {"name": "Logo071.scale-100", "size":71},
      {"name": "Logo071.scale-140", "size":99},
      {"name": "Logo071.scale-240", "size":170},

      {"name": "Logo150.scale-100", "size":150},
      {"name": "Logo150.scale-140", "size":210},
      {"name": "Logo150.scale-240", "size":360},

      {"name": "LogoStore.scale-100", "size":50},
      {"name": "LogoStore.scale-140", "size":70},
      {"name": "LogoStore.scale-240", "size":120},

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

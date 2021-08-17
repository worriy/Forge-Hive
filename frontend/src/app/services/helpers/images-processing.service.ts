import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ImagesProcessingHelper {
  
  /**
   * method called when the user select a new picture
   * @param  $event : the event of selecting a new picture
   */
  public imageLoaded(imageData: string, target, callback) : void
  {
    if(imageData)
    {

      var imageFile = this.dataURLtoBlob(imageData);

      //keep the type of the image to prepare canvas.drawImage
      var selectedImgType = imageFile.type;
      var reader = new FileReader();

      //listener on the onload event of the reader, triggered by the read.readAsDataUrl call
      reader.onload = (readerEvent:any) => 
      {
        //get the full size image
        var bigImg = readerEvent.target.result;
        
        //get the full size image size
        var bigSize = this.getImageSize(bigImg);

        //call resizeImage method to resize the full size image, 
        this.resizeImage(bigImg, selectedImgType, target, (imgUrl, imgName) => 
        {
          //get the resized image
          var smallImg = imgUrl;

          //get the resized image size
          var smallSize = this.getImageSize(smallImg);

          //get the blob from the new base64 image
          var blob = this.dataURLtoBlob(smallImg);

          //create a File instance from this blob
          var fileI = new File([blob], imgName);

          //if the resize image is heavier than 2Mo, it's too big, abort
          //TODO: find a way to check that BEFORE resizing the image to avoid the wait time before seeing the alert
          if(smallSize > 2)
            callback("tooBig");
          //if the size is ok
          else    
            callback(smallImg);
        });
      }
      //convert the image file to a base64 image and trigger the onload listener
      reader.readAsDataURL(imageFile);
    }
  }

  /**
   * Returns an image size (in Mo)
   * @param dataBase64 Base64 encoded image
   */
  private getImageSize(dataBase64)
  {
    //calculate the image size in Mo depending on its base64 encoding
    var head = 'data:image/jpeg;base64,';
    return ((dataBase64.length - head.length) * 3 / 4 / (1024*1024));
  }

  /**
   * Resize the image
   * @param readerEvent 
   */
  private resizeImage(img, type, target, callback)
  {

    //create the canvas and Image necessary
    var canvas: any = document.createElement("canvas");
    var image = new Image();

    //image onload listener triggered by the image.src = img call
    image.onload = () =>
    {
      var maxW = 0;
      var maxH = 0;
      //fixed max width and height depending if the image is a profile image or a card image
      if(target == 0 )
      {
        maxW = 800;
        maxH = 800;
      }
      else
      {
        maxW = 240;
        maxH = 240;
      }
      

      //get the image actual width and height
      var width = image.width;
      var height = image.height;

      //if the width is bigger than the height, calculate the final width and height depending on it
      if (width > height) 
      {
        if (width > maxW) 
        {
          height *= maxW / width;
          width = maxW;
        }
      } 
      //else, calculate depending on the height
      else 
      {
        if (height > maxH) 
        {
          width *= maxH / height;
          height = maxH;
        }
      }

      //set the canvas width and height with those values
      canvas.width = width;
      canvas.height = height;
      var ctx = canvas.getContext("2d");

      //draw the complete image but with the new size
      ctx.drawImage(image, 0, 0, image.width, image.height, 0, 0, canvas.width, canvas.height);

      var dataUrl = ""
      //if the image si a png
      if(type == "image/png")
        dataUrl = canvas.toDataURL('image/png', 0.9);
      //if it's a jpg or jpeg
      else  
        dataUrl = canvas.toDataURL('image/jpeg', 0.9);

      

      //trigger the callback method with the resized image 
      callback(dataUrl, image.name);
    }

    //set the Image instance image with our image, trigger the image.onload listener
    image.src = img;
  }

  /**
   * Create a Blob instance from a base64 image
   * @param dataurl 
   */
  private dataURLtoBlob(dataurl: any): Blob 
  {
    var arr = dataurl.split(','), mime = arr[0].match(/:(.*?);/)[1],
    bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
    while(n--)
    {
      u8arr[n] = bstr.charCodeAt(n);
    } 
    return new Blob([u8arr], {type:mime});
  }
}

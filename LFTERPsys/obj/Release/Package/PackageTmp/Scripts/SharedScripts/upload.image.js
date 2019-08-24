$(document).ready(function () {

    var readURL = function (input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('.avatar').attr('src', e.target.result);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }
    $(".file-upload").on('change', function () {
        

        var typ = document.getElementById("uploadedphoto").value;
        var res = typ.match(".jp");

        if (res) {
            readURL(this);
        }
        else {
            alert("Sorry only jpg/jpeg images are accepted");
            document.getElementById("uploadedphoto").value = "";
            $('#profileimage').attr('src', "/Images/ProfileImageAvatar/avatar.png");
        }
       
    });

    $("#profileimage").click(function (e) {
        $("#uploadedphoto").click();
    });
});

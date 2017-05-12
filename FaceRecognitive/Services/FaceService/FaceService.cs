using FaceRecognitive.Engine.Storage;
using FaceRecognitive.Infoes.FaceInfo;
using FaceRecognitive.Interfaces.FaceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceRecognitive.Services.FaceService
{
    public class FaceService : IFaceService
    {
        public FaceService()
        {
        }

        public List<FaceInfo> CallFaces(string username)
        {
            List<FaceInfo> result = null;

            if (username == "ALL_USERS")
            {
                result = VirtualFaceDatabase.Faces;
            }
            else
            {
                result = VirtualFaceDatabase.Faces.Where(x => x.Label == username).ToList();
            }

            return result;
        }

        public bool DeleteUser(string username)
        {
            throw new NotImplementedException();
        }

        public int GenerateUserId()
        {
            var date = DateTime.Now.ToString("MMddHHmmss");
            return Convert.ToInt32(date);
        }

        public List<string> GetAllUsernames()
        {
            return VirtualFaceDatabase.Faces.Select(x => x.Label).ToList();
        }

        public int GetUserId(string username)
        {
            int userId = 0;
            if (VirtualFaceDatabase.Faces.Any(x => x.Label == username))
            {
                userId = VirtualFaceDatabase.Faces.Where(x => x.Label == username).FirstOrDefault().UserId;
            }

            return userId;
        }

        public string GetUsername(int userId)
        {
            string username = string.Empty;
            if (VirtualFaceDatabase.Faces.Any(x => x.UserId == userId))
            {
                username = VirtualFaceDatabase.Faces.Where(x => x.UserId == userId).FirstOrDefault().Label;
            }

            return username;
        }

        public bool IsUsernameValid(string username)
        {
            throw new NotImplementedException();
        }

        public string SaveAdmin(string username, string password)
        {
            throw new NotImplementedException();
        }

        public string SaveFace(string username, byte[] faceBlob)
        {
            try
            {
                var exisitingUserId = GetUserId(username);

                if (exisitingUserId == 0)
                {
                    exisitingUserId = GenerateUserId();
                }

                var id = VirtualFaceDatabase.Faces.Count();
                var faceInfo = new FaceInfo
                {
                    Id = id,
                    Image = faceBlob,
                    Label = username,
                    UserId = exisitingUserId
                };

                VirtualFaceDatabase.Faces.Add(faceInfo);

                return String.Format("{0} face(s) saved successfully", username);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

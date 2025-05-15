using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Models;
using System.Text.RegularExpressions;

namespace SocialNetwork.Client
{
    class Program
    {
        private static SocialNetworkDbContext context;
        private static User currentUser;

        static void Main(string[] args)
        {
            // Initialize the database context
            context = new SocialNetworkDbContext();
            context.Database.EnsureCreated();

            Console.WriteLine("Welcome to the Social Network!");
            Console.WriteLine("Available commands:");
            PrintAvailableCommands();

            while (true)
            {
                Console.Write("\nEnter command: ");
                string input = Console.ReadLine();
                string[] tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length == 0)
                {
                    continue;
                }

                string command = tokens[0].ToLower();

                try
                {
                    ProcessCommand(command, tokens);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private static void PrintAvailableCommands()
        {
            if (currentUser == null)
            {
                Console.WriteLine("- Register <username> <password> <email>");
                Console.WriteLine("- Login <username> <password>");
                Console.WriteLine("- Exit");
            }
            else
            {
                Console.WriteLine("- Logout");
                Console.WriteLine("- CreateAlbum <album name> <is public> <background color>");
                Console.WriteLine("- AddPicture <album id> <picture title> <picture caption> <picture path>");
                Console.WriteLine("- ShareAlbum <album id> <username> <role: Owner/Viewer>");
                Console.WriteLine("- AddFriend <username>");
                Console.WriteLine("- AddTag <album id> <tag>");
                Console.WriteLine("- ListFriends");
                Console.WriteLine("- ListAlbums");
                Console.WriteLine("- ListUserAlbums <username>");
                Console.WriteLine("- ListAlbumsByTag <tag>");
                Console.WriteLine("- ListUsersWithMoreThan5Friends");
                Console.WriteLine("- Exit");
            }
        }

        private static void ProcessCommand(string command, string[] tokens)
        {
            switch (command)
            {
                case "register":
                    RegisterUser(tokens);
                    break;
                case "login":
                    LoginUser(tokens);
                    break;
                case "logout":
                    LogoutUser();
                    break;
                case "createalbum":
                    CreateAlbum(tokens);
                    break;
                case "addpicture":
                    AddPicture(tokens);
                    break;
                case "sharealbum":
                    ShareAlbum(tokens);
                    break;
                case "addfriend":
                    AddFriend(tokens);
                    break;
                case "addtag":
                    AddTag(tokens);
                    break;
                case "listfriends":
                    ListFriends();
                    break;
                case "listalbums":
                    ListAlbums();
                    break;
                case "listuseralbums":
                    ListUserAlbums(tokens);
                    break;
                case "listalbumsbytag":
                    ListAlbumsByTag(tokens);
                    break;
                case "listuserswithorethan5friends":
                    ListUsersWithMoreThan5Friends();
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Unknown command. Please try again.");
                    PrintAvailableCommands();
                    break;
            }
        }

        private static void RegisterUser(string[] tokens)
        {
            if (currentUser != null)
            {
                Console.WriteLine("You must logout first!");
                return;
            }

            if (tokens.Length < 4)
            {
                Console.WriteLine("Invalid command. Usage: Register <username> <password> <email>");
                return;
            }

            string username = tokens[1];
            string password = tokens[2];
            string email = tokens[3];

            // Validate username
            if (!Regex.IsMatch(username, @"^[a-zA-Z][a-zA-Z0-9]{2,}$"))
            {
                Console.WriteLine("Incorrect username");
                return;
            }

            // Validate password
            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+<>?])[A-Za-z\d!@#$%^&*()_+<>?]{6,50}$"))
            {
                Console.WriteLine("Incorrect password");
                return;
            }

            // Validate email
            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9]+([._-][a-zA-Z0-9]+)*@[a-zA-Z0-9]+([.-][a-zA-Z0-9]+)*\.[a-zA-Z]{2,}$"))
            {
                Console.WriteLine("Incorrect email");
                return;
            }

            // Check if username already exists
            if (context.Users.Any(u => u.Username == username))
            {
                Console.WriteLine("Username already exists!");
                return;
            }

            // Create new user
            User newUser = new User
            {
                Username = username,
                Password = password,
                Email = email,
                RegisteredOn = DateTime.Now,
                IsDeleted = false
            };

            context.Users.Add(newUser);
            context.SaveChanges();

            Console.WriteLine($"{username} was registered in the system");
        }

        private static void LoginUser(string[] tokens)
        {
            if (currentUser != null)
            {
                Console.WriteLine("You are already logged in!");
                return;
            }

            if (tokens.Length < 3)
            {
                Console.WriteLine("Invalid command. Usage: Login <username> <password>");
                return;
            }

            string username = tokens[1];
            string password = tokens[2];

            var user = context.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password && !u.IsDeleted);

            if (user == null)
            {
                Console.WriteLine("Incorrect username / password");
                return;
            }

            currentUser = user;
            user.LastTimeLoggedIn = DateTime.Now;
            context.SaveChanges();

            Console.WriteLine($"Successfully logged in {username}");
            PrintAvailableCommands();
        }

        private static void LogoutUser()
        {
            if (currentUser == null)
            {
                Console.WriteLine("Cannot log out. No user was logged in.");
                return;
            }

            string username = currentUser.Username;
            currentUser = null;

            Console.WriteLine($"User {username} successfully logged out");
            PrintAvailableCommands();
        }

        private static void CreateAlbum(string[] tokens)
        {
            EnsureUserIsLoggedIn();

            if (tokens.Length < 4)
            {
                Console.WriteLine("Invalid command. Usage: CreateAlbum <album name> <is public> <background color>");
                return;
            }

            string albumName = tokens[1];
            bool isPublic;

            if (!bool.TryParse(tokens[2], out isPublic))
            {
                Console.WriteLine("Invalid value for isPublic parameter. Use 'true' or 'false'.");
                return;
            }

            string backgroundColor = tokens[3];

            Album album = new Album
            {
                Name = albumName,
                IsPublic = isPublic,
                BackgroundColor = backgroundColor,
                OwnerId = currentUser.Id
            };

            context.Albums.Add(album);

            // Also add the owner to the AlbumUsers table with Owner role
            AlbumUser albumUser = new AlbumUser
            {
                Album = album,
                UserId = currentUser.Id,
                Role = Role.Owner
            };

            context.AlbumUsers.Add(albumUser);
            context.SaveChanges();

            Console.WriteLine($"Album '{albumName}' created successfully with ID: {album.Id}");
        }

        private static void AddPicture(string[] tokens)
        {
            EnsureUserIsLoggedIn();

            if (tokens.Length < 5)
            {
                Console.WriteLine("Invalid command. Usage: AddPicture <album id> <picture title> <picture caption> <picture path>");
                return;
            }

            if (!int.TryParse(tokens[1], out int albumId))
            {
                Console.WriteLine("Invalid album ID");
                return;
            }

            string pictureTitle = tokens[2];
            string pictureCaption = tokens[3];
            string picturePath = tokens[4];

            // Check if album exists and user has permission
            var album = context.Albums
                .Include(a => a.Users)
                .FirstOrDefault(a => a.Id == albumId);

            if (album == null)
            {
                Console.WriteLine("Album not found");
                return;
            }

            var albumUser = album.Users.FirstOrDefault(au => au.UserId == currentUser.Id);

            if (albumUser == null || albumUser.Role != Role.Owner)
            {
                Console.WriteLine("You don't have permission to add pictures to this album");
                return;
            }

            // Create new picture
            Picture picture = new Picture
            {
                Title = pictureTitle,
                Caption = pictureCaption,
                Path = picturePath
            };

            context.Pictures.Add(picture);
            context.SaveChanges();

            // Add picture to album
            AlbumPicture albumPicture = new AlbumPicture
            {
                AlbumId = albumId,
                PictureId = picture.Id
            };

            context.AlbumPictures.Add(albumPicture);
            context.SaveChanges();

            Console.WriteLine($"Picture '{pictureTitle}' added to album '{album.Name}'");
        }

        private static void ShareAlbum(string[] tokens)
        {
            EnsureUserIsLoggedIn();

            if (tokens.Length < 4)
            {
                Console.WriteLine("Invalid command. Usage: ShareAlbum <album id> <username> <role: Owner/Viewer>");
                return;
            }

            if (!int.TryParse(tokens[1], out int albumId))
            {
                Console.WriteLine("Invalid album ID");
                return;
            }

            string username = tokens[2];
            string roleStr = tokens[3];

            // Parse role
            Role role;
            if (!Enum.TryParse(roleStr, true, out role))
            {
                Console.WriteLine("Invalid role. Use 'Owner' or 'Viewer'");
                return;
            }

            // Check if album exists and user has permission
            var album = context.Albums
                .Include(a => a.Users)
                .FirstOrDefault(a => a.Id == albumId);

            if (album == null)
            {
                Console.WriteLine("Album not found");
                return;
            }

            var albumUser = album.Users.FirstOrDefault(au => au.UserId == currentUser.Id);

            if (albumUser == null || albumUser.Role != Role.Owner)
            {
                Console.WriteLine("You don't have permission to share this album");
                return;
            }

            // Find user to share with
            var targetUser = context.Users
                .FirstOrDefault(u => u.Username == username && !u.IsDeleted);

            if (targetUser == null)
            {
                Console.WriteLine("User not found");
                return;
            }

            // Check if album is already shared with this user
            if (album.Users.Any(au => au.UserId == targetUser.Id))
            {
                Console.WriteLine("Album is already shared with this user");
                return;
            }

            // Share album with user
            AlbumUser newAlbumUser = new AlbumUser
            {
                AlbumId = albumId,
                UserId = targetUser.Id,
                Role = role
            };

            context.AlbumUsers.Add(newAlbumUser);
            context.SaveChanges();

            Console.WriteLine($"Album '{album.Name}' shared with user '{username}' as {role}");
        }

        private static void AddFriend(string[] tokens)
        {
            EnsureUserIsLoggedIn();

            if (tokens.Length < 2)
            {
                Console.WriteLine("Invalid command. Usage: AddFriend <username>");
                return;
            }

            string friendUsername = tokens[1];

            // Check if friend exists
            var friend = context.Users
                .FirstOrDefault(u => u.Username == friendUsername && !u.IsDeleted);

            if (friend == null)
            {
                Console.WriteLine("User not found");
                return;
            }

            // Check if already friends
            if (context.UserFriends.Any(uf =>
                (uf.UserId == currentUser.Id && uf.FriendId == friend.Id) ||
                (uf.UserId == friend.Id && uf.FriendId == currentUser.Id)))
            {
                Console.WriteLine("You are already friends with this user");
                return;
            }

            // Add friend relationship (both ways)
            UserFriend userFriend1 = new UserFriend
            {
                UserId = currentUser.Id,
                FriendId = friend.Id
            };

            UserFriend userFriend2 = new UserFriend
            {
                UserId = friend.Id,
                FriendId = currentUser.Id
            };

            context.UserFriends.Add(userFriend1);
            context.UserFriends.Add(userFriend2);
            context.SaveChanges();

            Console.WriteLine($"You are now friends with {friendUsername}");
        }

        private static void AddTag(string[] tokens)
        {
            EnsureUserIsLoggedIn();

            if (tokens.Length < 3)
            {
                Console.WriteLine("Invalid command. Usage: AddTag <album id> <tag>");
                return;
            }

            if (!int.TryParse(tokens[1], out int albumId))
            {
                Console.WriteLine("Invalid album ID");
                return;
            }

            string tagInput = tokens[2];

            // Transform the tag to valid format
            string transformedTag = TagTransformer.Transform(tagInput);

            // Check if album exists and user has permission
            var album = context.Albums
                .Include(a => a.Users)
                .FirstOrDefault(a => a.Id == albumId);

            if (album == null)
            {
                Console.WriteLine("Album not found");
                return;
            }

            var albumUser = album.Users.FirstOrDefault(au => au.UserId == currentUser.Id);

            if (albumUser == null || albumUser.Role != Role.Owner)
            {
                Console.WriteLine("You don't have permission to add tags to this album");
                return;
            }

            // Check if tag already exists
            var tag = context.Tags
                .FirstOrDefault(t => t.Name == transformedTag);

            if (tag == null)
            {
                // Create new tag
                tag = new Tag
                {
                    Name = transformedTag
                };

                context.Tags.Add(tag);
                context.SaveChanges();
            }

            // Check if album already has this tag
            if (context.AlbumTags.Any(at => at.AlbumId == albumId && at.TagId == tag.Id))
            {
                Console.WriteLine($"Album already has tag {transformedTag}");
                return;
            }

            // Add tag to album
            AlbumTag albumTag = new AlbumTag
            {
                AlbumId = albumId,
                TagId = tag.Id
            };

            context.AlbumTags.Add(albumTag);
            context.SaveChanges();

            Console.WriteLine($"{transformedTag} was added to database");
        }

        private static void ListFriends()
        {
            EnsureUserIsLoggedIn();

            var friends = context.UserFriends
                .Include(uf => uf.Friend)
                .Where(uf => uf.UserId == currentUser.Id)
                .OrderBy(uf => uf.Friend.Username)
                .ToList();

            if (friends.Count == 0)
            {
                Console.WriteLine("You have no friends yet");
                return;
            }

            Console.WriteLine($"Friends of {currentUser.Username}:");
            foreach (var uf in friends)
            {
                string status = uf.Friend.IsDeleted ? "Inactive" : "Active";
                Console.WriteLine($"- {uf.Friend.Username} - {status}");
            }
        }

        private static void ListAlbums()
        {
            EnsureUserIsLoggedIn();

            var albums = context.Albums
                .Include(a => a.Owner)
                .Include(a => a.Pictures)
                    .ThenInclude(ap => ap.Picture)
                .Where(a => a.Users.Any(au => au.UserId == currentUser.Id))
                .OrderByDescending(a => a.Pictures.Count)
                .ThenBy(a => a.Name)
                .ToList();

            if (albums.Count == 0)
            {
                Console.WriteLine("You have no albums yet");
                return;
            }

            Console.WriteLine($"Albums for {currentUser.Username}:");
            foreach (var album in albums)
            {
                Console.WriteLine($"- {album.Name} (Owner: {album.Owner.Username}, Pictures: {album.Pictures.Count})");

                // If album is not public, don't show its contents
                if (!album.IsPublic && album.OwnerId != currentUser.Id)
                {
                    Console.WriteLine("  Private content!");
                    continue;
                }

                // List pictures in album
                foreach (var ap in album.Pictures)
                {
                    Console.WriteLine($"  > {ap.Picture.Title} - {ap.Picture.Path}");
                }
            }
        }

        private static void ListUserAlbums(string[] tokens)
        {
            EnsureUserIsLoggedIn();

            if (tokens.Length < 2)
            {
                Console.WriteLine("Invalid command. Usage: ListUserAlbums <username>");
                return;
            }

            string username = tokens[1];

            // Find user
            var user = context.Users
                .FirstOrDefault(u => u.Username == username && !u.IsDeleted);

            if (user == null)
            {
                Console.WriteLine("User not found");
                return;
            }

            var albums = context.Albums
                .Include(a => a.Pictures)
                    .ThenInclude(ap => ap.Picture)
                .Where(a => a.OwnerId == user.Id)
                .OrderBy(a => a.Name)
                .ToList();

            if (albums.Count == 0)
            {
                Console.WriteLine($"{username} has no albums");
                return;
            }

            Console.WriteLine($"Albums for {username}:");
            foreach (var album in albums)
            {
                Console.WriteLine($"- {album.Name}");

                // If album is not public and current user is not the owner, don't show details
                if (!album.IsPublic && album.OwnerId != currentUser.Id)
                {
                    Console.WriteLine("  Private content!");
                    continue;
                }

                // List pictures in album
                foreach (var ap in album.Pictures)
                {
                    Console.WriteLine($"  > {ap.Picture.Title} - {ap.Picture.Path}");
                }
            }
        }

        private static void ListAlbumsByTag(string[] tokens)
        {
            EnsureUserIsLoggedIn();

            if (tokens.Length < 2)
            {
                Console.WriteLine("Invalid command. Usage: ListAlbumsByTag <tag>");
                return;
            }

            string tagName = tokens[1];

            // Make sure tag starts with #
            if (!tagName.StartsWith("#"))
            {
                tagName = "#" + tagName;
            }

            var albums = context.AlbumTags
                .Include(at => at.Album)
                    .ThenInclude(a => a.Owner)
                .Include(at => at.Tag)
                .Where(at => at.Tag.Name == tagName)
                .OrderByDescending(at => at.Album.AlbumTags.Count)
                .ThenBy(at => at.Album.Name)
                .Select(at => at.Album)
                .ToList();

            if (albums.Count == 0)
            {
                Console.WriteLine($"No albums found with tag {tagName}");
                return;
            }

            Console.WriteLine($"Albums with tag {tagName}:");
            foreach (var album in albums)
            {
                Console.WriteLine($"- {album.Name} (Owner: {album.Owner.Username})");
            }
        }

        private static void ListUsersWithMoreThan5Friends()
        {
            EnsureUserIsLoggedIn();

            var usersWithMoreThan5Friends = context.Users
                .Where(u => !u.IsDeleted)
                .Select(u => new
                {
                    User = u,
                    FriendsCount = u.Friends.Count
                })
                .Where(x => x.FriendsCount > 5)
                .OrderBy(x => x.User.RegisteredOn)
                .ThenByDescending(x => x.FriendsCount)
                .ToList();

            if (usersWithMoreThan5Friends.Count == 0)
            {
                Console.WriteLine("No users found with more than 5 friends");
                return;
            }

            Console.WriteLine("Users with more than 5 friends:");
            foreach (var item in usersWithMoreThan5Friends)
            {
                int daysSinceRegistration = (DateTime.Now - item.User.RegisteredOn).Days;
                Console.WriteLine($"- {item.User.Username} - Friends: {item.FriendsCount}, Member for {daysSinceRegistration} days");
            }
        }

        private static void EnsureUserIsLoggedIn()
        {
            if (currentUser == null)
            {
                throw new InvalidOperationException("You must be logged in to perform this action");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GettingRealWPF.Models;

namespace GettingRealWPF.Repositories;
public class FileUserRepository
{
    private readonly string filePath;

    public FileUserRepository(string filePath)
    {
        this.filePath = filePath;
    }

    public List<User> GetAllUsers()
    {
        var users = new List<User>();
        if (File.Exists(filePath))
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split(';');
                if (parts.Length == 2)
                {
                    users.Add(new User
                    {
                        Name = parts[0],
                        Password = parts[1]
                    });
                }
            }
        }
        return users;
    }

    public void AddUser(User user)
    {
        using (var writer = File.AppendText(filePath))
        {
            writer.WriteLine($"{user.Name};{user.Password}");
        }
    }

    public void EditUser(User updatedUser)
    {
        var users = GetAllUsers();
        var index = users.FindIndex(u => u.Name == updatedUser.Name);
        if (index >= 0)
        {
            users[index] = updatedUser;
            File.WriteAllLines(filePath, users.Select(u => $"{u.Name};{u.Password}"));
        }
    }

    public void DeleteUser(User user)
    {
        var users = GetAllUsers();
        users.RemoveAll(u => u.Name == user.Name);
        File.WriteAllLines(filePath, users.Select(u => $"{u.Name};{u.Password}"));
    }
}
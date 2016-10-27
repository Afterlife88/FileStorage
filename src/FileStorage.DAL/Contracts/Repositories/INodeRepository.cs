﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorage.Domain.Entities;

namespace FileStorage.DAL.Contracts.Repositories
{
    public interface INodeRepository
    {
        void AddNode(Node node);
        Task<Node> GetNodeByIdAsync(Guid nodeId);
        Task<Node> GetNodeByNameAsync(string nodeName);
        Task<IEnumerable<Node>> GetAllNodesForUserAsync(string userId);
        Task<Node> GetRootFolderForUserAsync(string userId);
        Node RenameNode(Node node, string newName);
    }
}
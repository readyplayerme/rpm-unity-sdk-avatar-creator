using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ReadyPlayerMe.AvatarCreator
{
    public class TemplateFetcher
    {
        private readonly CancellationToken ctx;
        private readonly AvatarAPIRequests avatarAPIRequests;
        private readonly List<TemplateData> templates;

        public TemplateFetcher(CancellationToken ctx = default)
        {
            this.ctx = ctx;
            avatarAPIRequests = new AvatarAPIRequests(ctx);
            templates = new List<TemplateData>();
        }

        public async Task<List<TemplateData>> GetTemplates()
        {
            var avatarTemplates = await avatarAPIRequests.GetTemplates();
            await GetAllTemplateRenders(avatarTemplates);
            return templates;
        }

        private async Task GetAllTemplateRenders(IEnumerable<TemplateData> templateAvatars)
        {
            var downloadRenderTasks = templateAvatars.Select(GetAvatarRender).ToList();

            while (!downloadRenderTasks.All(x => x.IsCompleted) && !ctx.IsCancellationRequested)
            {
                await Task.Yield();
            }
        }

        private async Task GetAvatarRender(TemplateData templateData)
        {
            try
            {
                templateData.Texture = await avatarAPIRequests.GetTemplateAvatarImage(templateData.ImageUrl);
                templates.Add(templateData);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

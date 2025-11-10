define(['baseView', 'loading', 'toast'], function (BaseView, loading, toast) {
    'use strict';

    const pluginId = '12345678-1234-1234-1234-123456789012';

    class EmbyTagsConfigPage extends BaseView {
        constructor(view, params) {
            super(view, params);
            this.view = view;
        }

        onResume(options) {
            super.onResume(options);
            this.loadConfiguration();
            this.bindEvents();
        }

        loadConfiguration() {
            const page = this.view;
            loading.show();

            ApiClient.getPluginConfiguration(pluginId).then(function (config) {
                page.querySelector('#txtSonarrApiUrl').value = config.SonarrApiUrl || '';
                page.querySelector('#txtSonarrApiKey').value = config.SonarrApiKey || '';
                page.querySelector('#txtTagPrefix').value = config.TagPrefix || '';
                page.querySelector('#selectSyncInterval').value = config.SyncIntervalHours || 24;
                page.querySelector('#chkAutoSync').checked = config.AutoSync || false;
                page.querySelector('#chkOverwriteExistingTags').checked = config.OverwriteExistingTags || false;
                page.querySelector('#chkDryRun').checked = config.DryRun || false;
                page.querySelector('#chkDebugMode').checked = config.EnableDebugLogging || false;
                loading.hide();
            }).catch(function (err) {
                loading.hide();
                toast({ text: 'Error loading configuration', type: 'error' });
            });
        }

        bindEvents() {
            const page = this.view;
            const form = page.querySelector('.embySonarrTagsConfigurationForm');
            
            if (form && !form.dataset.bound) {
                form.dataset.bound = 'true';
                
                form.addEventListener('submit', (e) => {
                    e.preventDefault();
                    this.saveConfiguration();
                    return false;
                });
            }

            const syncBtn = page.querySelector('#btnSyncNow');
            if (syncBtn && !syncBtn.dataset.bound) {
                syncBtn.dataset.bound = 'true';
                syncBtn.addEventListener('click', () => {
                    this.syncNow();
                });
            }
        }

        saveConfiguration() {
            const page = this.view;
            loading.show();

            ApiClient.getPluginConfiguration(pluginId).then(function (config) {
                config.SonarrApiUrl = page.querySelector('#txtSonarrApiUrl').value;
                config.SonarrApiKey = page.querySelector('#txtSonarrApiKey').value;
                config.TagPrefix = page.querySelector('#txtTagPrefix').value;
                config.SyncIntervalHours = parseInt(page.querySelector('#selectSyncInterval').value);
                config.AutoSync = page.querySelector('#chkAutoSync').checked;
                config.OverwriteExistingTags = page.querySelector('#chkOverwriteExistingTags').checked;
                config.DryRun = page.querySelector('#chkDryRun').checked;
                config.EnableDebugLogging = page.querySelector('#chkDebugMode').checked;

                return ApiClient.updatePluginConfiguration(pluginId, config);
            }).then(function () {
                loading.hide();
                toast({ text: 'Settings saved successfully. Check server logs for connection test results.' });
            }).catch(function (err) {
                loading.hide();
                toast({ text: 'Error saving configuration', type: 'error' });
            });
        }

        syncNow() {
            const page = this.view;
            const dryRun = page.querySelector('#chkDryRun').checked;
            loading.show();

            ApiClient.ajax({
                type: 'POST',
                url: ApiClient.getUrl('EmbyTags/Sync?DryRun=' + dryRun)
            }).then(function (result) {
                loading.hide();
                let msg = result.Message || 'Sync completed';
                if (result.DryRun) {
                    msg += ' (DRY RUN - No changes were made)';
                }
                Dashboard.alert(msg);
            }).catch(function (err) {
                loading.hide();
                Dashboard.alert('Sync failed. Check the server logs for details.');
            });
        }
    }

    return EmbyTagsConfigPage;
});

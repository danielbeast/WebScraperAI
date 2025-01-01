﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', () => {
    const elements = {
        url: document.getElementById('url'),
        prompt: document.getElementById('prompt'),
        extractBtn: document.getElementById('extractBtn'),
        results: document.getElementById('results'),
        loadingSpinner: document.getElementById('loadingSpinner'),
        resultsSection: document.getElementById('resultsSection')
    };

    const showLoading = () => elements.loadingSpinner.classList.remove('hidden');
    const hideLoading = () => elements.loadingSpinner.classList.add('hidden');

    const showError = (message) => {
        alert(message);
        hideLoading();
    };

    const formatResult = (result) => {
        try {
            // If result is already a JSON string, parse it
            const parsed = typeof result === 'string' ? JSON.parse(result) : result;
            return JSON.stringify(parsed, null, 2);
        } catch (e) {
            return result; // Return as is if parsing fails
        }
    };

    elements.extractBtn.addEventListener('click', async () => {
        const url = elements.url.value.trim();
        const prompt = elements.prompt.value.trim();

        if (!url || !prompt) {
            showError('Please enter both URL and extraction prompt');
            return;
        }

        showLoading();
        try {
            const response = await axios.post('/scrape-and-parse', {
                url: url,
                parseDescription: prompt
            });

            // Only display the result part, properly formatted
            const formattedResult = formatResult(response.data.data);
            elements.results.textContent = formattedResult;
            elements.resultsSection.classList.remove('hidden');
            
        } catch (error) {
            showError(error.response?.data?.error || 'Failed to extract information');
        } finally {
            hideLoading();
        }
    });
});
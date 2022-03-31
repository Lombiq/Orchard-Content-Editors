const recommendedSetup = require('../../../Utilities/Lombiq.Gulp.Extensions/recommended-setup');

recommendedSetup.setupRecommendedScssAndJsTasksAndVendorsCopyAssets([
    {
        name: 'vue-router',
        path: './node_modules/vue-router/dist/**',
    }]);

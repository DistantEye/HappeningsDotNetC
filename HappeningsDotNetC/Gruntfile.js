/// <binding BeforeBuild='clean' AfterBuild='copy' />
module.exports = function (grunt) {
    grunt.initConfig({
        clean: ["wwwroot/lib/*", "temp/"],
        copy: {            
            main: {                
                files: [
                    // original files that used to be statically sitting in project solution
                    {
                        cwd: 'node_modules/bootstrap/dist',
                        src: '**/*', 
                        dest: 'wwwroot/lib/bootstrap/dist',
                        expand: true
                    },
                    {
                        cwd: 'node_modules/jquery/dist',
                        src: 'jquery.js', 
                        dest: 'wwwroot/lib/jquery/dist',
                        expand: true
                    },
                    {
                        cwd: 'node_modules/jquery-validation/dist',
                        src: '**/*', 
                        dest: 'wwwroot/lib/jquery-validation/dist',
                        expand: true
                    },
                    {
                        cwd: 'node_modules/jquery-validation-unobtrusive/dist',
                        src: '**/*', 
                        dest: 'wwwroot/lib/jquery-validation-unobtrusive',
                        expand: true
                    },
                    // new stuff
                    {
                        cwd: 'node_modules/moment',
                        src: '**/*', 
                        dest: 'wwwroot/lib/moment',
                        expand: true
                    },
                    {
                        cwd: 'node_modules/eonasdan-bootstrap-datetimepicker/src',
                        src: '**/*',
                        dest: 'wwwroot/lib/eonasdan-bootstrap-datetimepicker/src/',
                        expand: true
                    },
                    {
                        cwd: 'node_modules/eonasdan-bootstrap-datetimepicker/build',
                        src: '**/*', 
                        dest: 'wwwroot/lib/eonasdan-bootstrap-datetimepicker/build/',
                        expand: true
                    },
                ]
            }
        },
    });

    grunt.loadNpmTasks("grunt-contrib-clean");
    grunt.loadNpmTasks("grunt-contrib-copy");
};
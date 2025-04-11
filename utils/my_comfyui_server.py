from flask import Flask, request, Response, jsonify
import requests
import logging
import argparse
import os
import sys
import io

# 设置日志
logging.basicConfig(level=logging.INFO, 
                    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')
logger = logging.getLogger(__name__)

app = Flask(__name__)

# 默认ComfyUI服务器地址
COMFYUI_SERVER = "http://localhost:8188"

@app.route('/', defaults={'path': ''})
@app.route('/<path:path>', methods=['GET', 'POST', 'DELETE', 'PUT'])
def proxy(path):
    """通用代理，将所有请求转发到ComfyUI服务器"""
    target_url = f"{COMFYUI_SERVER}/{path}"
    logger.info(f"收到请求: {request.method} {path}")
    logger.info(f"转发到: {target_url}")
    
    # 准备请求头
    headers = {k: v for k, v in request.headers if k != 'Host'}
    
    try:
        # 处理POST请求
        if request.method == 'POST':
            # 处理文件上传 (特殊处理 /upload/image 路径)
            if path == 'upload/image' and request.files:
                # 直接提取文件对象
                image_file = request.files.get('image')
                if image_file:
                    # 直接使用requests的文件上传功能
                    file_content = image_file.read()
                    file_name = image_file.filename
                    
                    files = {'image': (file_name, file_content, image_file.content_type)}
                    response = requests.post(target_url, files=files)
                    
                    logger.info(f"上传图片响应状态: {response.status_code}")
                    return Response(
                        response=response.content,
                        status=response.status_code,
                        content_type=response.headers.get('content-type', 'application/json')
                    )
            # 处理其他表单数据上传
            elif request.files:
                files = {}
                for key, file in request.files.items():
                    files[key] = (file.filename, file.read(), file.content_type)
                
                data = request.form.to_dict() if request.form else None
                response = requests.post(target_url, files=files, data=data, headers=headers)
            else:
                # 处理JSON请求（如工作流提交）
                data = request.get_data()
                content_type = request.headers.get('Content-Type', '')
                
                # 设置适当的Content-Type
                if content_type:
                    headers['Content-Type'] = content_type
                
                response = requests.post(target_url, data=data, headers=headers)
        
        # 处理GET请求
        elif request.method == 'GET':
            response = requests.get(target_url, params=request.args, headers=headers, stream=True)
        
        # 处理其他方法
        else:
            data = request.get_data()
            response = requests.request(
                method=request.method,
                url=target_url,
                headers=headers,
                data=data,
                params=request.args
            )
        
        # 转发响应
        logger.info(f"收到响应: {response.status_code}")
        
        # 创建Flask响应对象
        content = response.content
        proxy_response = Response(
            response=content,
            status=response.status_code,
            content_type=response.headers.get('content-type', 'text/plain')
        )
        
        # 复制响应头
        for key, value in response.headers.items():
            if key.lower() not in ('content-encoding', 'content-length', 'transfer-encoding', 'connection'):
                proxy_response.headers[key] = value
                
        return proxy_response
        
    except requests.RequestException as e:
        logger.error(f"代理请求错误: {e}")
        return jsonify({"error": str(e)}), 500
    except Exception as e:
        logger.error(f"未知错误: {e}")
        import traceback
        traceback.print_exc()
        return jsonify({"error": str(e)}), 500

@app.route('/status', methods=['GET'])
def status():
    """检查与ComfyUI服务器的连接状态"""
    try:
        response = requests.get(f"{COMFYUI_SERVER}/system_stats")
        if response.status_code == 200:
            return jsonify({
                "status": "online",
                "comfyui_server": COMFYUI_SERVER,
                "proxy_server": "running"
            })
        else:
            return jsonify({
                "status": "error",
                "message": f"ComfyUI服务器返回错误代码: {response.status_code}",
                "comfyui_server": COMFYUI_SERVER,
                "proxy_server": "running"
            }), 500
    except requests.RequestException as e:
        return jsonify({
            "status": "error",
            "message": f"无法连接到ComfyUI服务器: {str(e)}",
            "comfyui_server": COMFYUI_SERVER,
            "proxy_server": "running"
        }), 500

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='ComfyUI代理服务器')
    parser.add_argument('--host', type=str, default='0.0.0.0', help='代理服务器监听地址')
    parser.add_argument('--port', type=int, default=8089, help='代理服务器端口')
    parser.add_argument('--comfyui', type=str, default='http://localhost:8188', help='ComfyUI服务器地址')
    parser.add_argument('--debug', action='store_true', help='启用调试模式')
    
    args = parser.parse_args()
    COMFYUI_SERVER = args.comfyui
    
    logger.info(f"启动代理服务器在 {args.host}:{args.port}")
    logger.info(f"转发请求到ComfyUI服务器: {COMFYUI_SERVER}")
    
    app.run(host=args.host, port=args.port, debug=args.debug)

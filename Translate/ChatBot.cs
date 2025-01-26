﻿using GenerativeAI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translate
{
    public class ChatBot
    {
        private readonly GenerativeModel model;
        private List<string> conversationHistory;

        // コンストラクタでAPIキーを渡し、APIクライアントを初期化
        public ChatBot()
        {
            string apiKey = ConfigurationManager.AppSettings["GeminiApiKey"];
            model = new GenerativeModel(apiKey);
            conversationHistory = new List<string>();
        }

        // チャットのメイン機能: ユーザーの入力に対してAPIの応答を取得
        public async Task<string> SendMessageAsync(string userMessage)
        {
            // 会話履歴にユーザーのメッセージを追加
            //conversationHistory.Add($"{userMessage}");

            // コンテキスト付きのプロンプトを作成
            //string promptWithContext = string.Join("\n", conversationHistory);

            try
            {
                // メッセージをGemini APIに送信し、応答を受け取る
                var response = await model.GenerateContentAsync(userMessage);

                // ボットの応答を履歴に追加
                //conversationHistory.Add($"{response}");

                // ボットの応答を返す
                return response;
            }
            catch (Exception ex)
            {
                // エラーが発生した場合の処理
                return $"リクエストエラー: {ex.Message}";
            }
        }
       
    }
}
